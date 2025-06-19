-- Database Migration Script for Password Security Enhancement
-- Run this script to add password hashing support to existing database

-- Add password_hash column to userTBL if it doesn't exist
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'userTBL' AND COLUMN_NAME = 'password_hash')
BEGIN
    ALTER TABLE userTBL ADD password_hash NVARCHAR(255) NULL;
    PRINT 'Added password_hash column to userTBL';
END
ELSE
BEGIN
    PRINT 'password_hash column already exists in userTBL';
END

-- Add security audit table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'security_audit')
BEGIN
    CREATE TABLE security_audit (
        id BIGINT IDENTITY(1,1) PRIMARY KEY,
        event_type NVARCHAR(50) NOT NULL,
        event_description NVARCHAR(500) NOT NULL,
        user_id INT NULL,
        ip_address NVARCHAR(45) NULL,
        user_agent NVARCHAR(500) NULL,
        created_date DATETIME2 DEFAULT GETDATE(),
        severity NVARCHAR(20) DEFAULT 'INFO'
    );
    
    CREATE INDEX IX_security_audit_date ON security_audit(created_date);
    CREATE INDEX IX_security_audit_user ON security_audit(user_id);
    CREATE INDEX IX_security_audit_ip ON security_audit(ip_address);
    
    PRINT 'Created security_audit table';
END
ELSE
BEGIN
    PRINT 'security_audit table already exists';
END

-- Add failed login attempts table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'failed_login_attempts')
BEGIN
    CREATE TABLE failed_login_attempts (
        id BIGINT IDENTITY(1,1) PRIMARY KEY,
        username NVARCHAR(50) NOT NULL,
        ip_address NVARCHAR(45) NOT NULL,
        attempt_time DATETIME2 DEFAULT GETDATE(),
        user_agent NVARCHAR(500) NULL
    );
    
    CREATE INDEX IX_failed_login_username ON failed_login_attempts(username);
    CREATE INDEX IX_failed_login_ip ON failed_login_attempts(ip_address);
    CREATE INDEX IX_failed_login_time ON failed_login_attempts(attempt_time);
    
    PRINT 'Created failed_login_attempts table';
END
ELSE
BEGIN
    PRINT 'failed_login_attempts table already exists';
END

-- Add session tracking table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'user_sessions')
BEGIN
    CREATE TABLE user_sessions (
        id BIGINT IDENTITY(1,1) PRIMARY KEY,
        user_id INT NOT NULL,
        session_token NVARCHAR(255) NOT NULL UNIQUE,
        ip_address NVARCHAR(45) NOT NULL,
        user_agent NVARCHAR(500) NULL,
        created_date DATETIME2 DEFAULT GETDATE(),
        last_activity DATETIME2 DEFAULT GETDATE(),
        is_active BIT DEFAULT 1,
        logout_date DATETIME2 NULL
    );
    
    CREATE INDEX IX_user_sessions_user ON user_sessions(user_id);
    CREATE INDEX IX_user_sessions_token ON user_sessions(session_token);
    CREATE INDEX IX_user_sessions_active ON user_sessions(is_active, last_activity);
    
    PRINT 'Created user_sessions table';
END
ELSE
BEGIN
    PRINT 'user_sessions table already exists';
END

-- Add password history table for password policy enforcement
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'password_history')
BEGIN
    CREATE TABLE password_history (
        id BIGINT IDENTITY(1,1) PRIMARY KEY,
        user_id INT NOT NULL,
        password_hash NVARCHAR(255) NOT NULL,
        created_date DATETIME2 DEFAULT GETDATE()
    );
    
    CREATE INDEX IX_password_history_user ON password_history(user_id);
    CREATE INDEX IX_password_history_date ON password_history(created_date);
    
    PRINT 'Created password_history table';
END
ELSE
BEGIN
    PRINT 'password_history table already exists';
END

-- Add stored procedures for security operations
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME = 'sp_LogSecurityEvent')
BEGIN
    EXEC('
    CREATE PROCEDURE sp_LogSecurityEvent
        @EventType NVARCHAR(50),
        @EventDescription NVARCHAR(500),
        @UserId INT = NULL,
        @IpAddress NVARCHAR(45) = NULL,
        @UserAgent NVARCHAR(500) = NULL,
        @Severity NVARCHAR(20) = ''INFO''
    AS
    BEGIN
        SET NOCOUNT ON;
        
        INSERT INTO security_audit (event_type, event_description, user_id, ip_address, user_agent, severity)
        VALUES (@EventType, @EventDescription, @UserId, @IpAddress, @UserAgent, @Severity);
    END
    ');
    PRINT 'Created sp_LogSecurityEvent stored procedure';
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME = 'sp_LogFailedLogin')
BEGIN
    EXEC('
    CREATE PROCEDURE sp_LogFailedLogin
        @Username NVARCHAR(50),
        @IpAddress NVARCHAR(45),
        @UserAgent NVARCHAR(500) = NULL
    AS
    BEGIN
        SET NOCOUNT ON;
        
        INSERT INTO failed_login_attempts (username, ip_address, user_agent)
        VALUES (@Username, @IpAddress, @UserAgent);
        
        -- Clean up old failed attempts (older than 24 hours)
        DELETE FROM failed_login_attempts 
        WHERE attempt_time < DATEADD(HOUR, -24, GETDATE());
    END
    ');
    PRINT 'Created sp_LogFailedLogin stored procedure';
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME = 'sp_GetFailedLoginCount')
BEGIN
    EXEC('
    CREATE PROCEDURE sp_GetFailedLoginCount
        @Username NVARCHAR(50),
        @TimeWindowMinutes INT = 15
    AS
    BEGIN
        SET NOCOUNT ON;
        
        SELECT COUNT(*) as FailedAttempts
        FROM failed_login_attempts 
        WHERE username = @Username 
        AND attempt_time > DATEADD(MINUTE, -@TimeWindowMinutes, GETDATE());
    END
    ');
    PRINT 'Created sp_GetFailedLoginCount stored procedure';
END

-- Add triggers for audit logging
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE name = 'tr_userTBL_audit')
BEGIN
    EXEC('
    CREATE TRIGGER tr_userTBL_audit
    ON userTBL
    AFTER UPDATE
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- Log password changes
        IF UPDATE(password_hash)
        BEGIN
            INSERT INTO security_audit (event_type, event_description, user_id, severity)
            SELECT ''PASSWORD_CHANGE'', ''Password changed for user: '' + i.username, i.userid, ''INFO''
            FROM inserted i
            INNER JOIN deleted d ON i.userid = d.userid
            WHERE ISNULL(i.password_hash, '''') <> ISNULL(d.password_hash, '''');
        END
        
        -- Log role changes
        IF UPDATE(role)
        BEGIN
            INSERT INTO security_audit (event_type, event_description, user_id, severity)
            SELECT ''ROLE_CHANGE'', ''Role changed from '' + d.role + '' to '' + i.role + '' for user: '' + i.username, i.userid, ''WARNING''
            FROM inserted i
            INNER JOIN deleted d ON i.userid = d.userid
            WHERE i.role <> d.role;
        END
        
        -- Log access level changes
        IF UPDATE(access)
        BEGIN
            INSERT INTO security_audit (event_type, event_description, user_id, severity)
            SELECT ''ACCESS_CHANGE'', ''Access level changed from '' + CAST(d.access AS NVARCHAR) + '' to '' + CAST(i.access AS NVARCHAR) + '' for user: '' + i.username, i.userid, ''WARNING''
            FROM inserted i
            INNER JOIN deleted d ON i.userid = d.userid
            WHERE i.access <> d.access;
        END
    END
    ');
    PRINT 'Created audit trigger for userTBL';
END

-- Create indexes for better performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_userTBL_username')
BEGIN
    CREATE INDEX IX_userTBL_username ON userTBL(username);
    PRINT 'Created index on userTBL.username';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_userTBL_password_hash')
BEGIN
    CREATE INDEX IX_userTBL_password_hash ON userTBL(password_hash);
    PRINT 'Created index on userTBL.password_hash';
END

-- Add constraints for data integrity
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.CHECK_CONSTRAINTS WHERE CONSTRAINT_NAME = 'CK_userTBL_access_valid')
BEGIN
    ALTER TABLE userTBL ADD CONSTRAINT CK_userTBL_access_valid CHECK (access IN (0, 1, 2, 3, 4));
    PRINT 'Added access level constraint to userTBL';
END

-- Clean up old sessions (run this periodically)
CREATE OR ALTER PROCEDURE sp_CleanupOldSessions
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Mark sessions as inactive if they haven't been active for more than 30 minutes
    UPDATE user_sessions 
    SET is_active = 0, logout_date = GETDATE()
    WHERE is_active = 1 
    AND last_activity < DATEADD(MINUTE, -30, GETDATE());
    
    -- Delete very old session records (older than 30 days)
    DELETE FROM user_sessions 
    WHERE created_date < DATEADD(DAY, -30, GETDATE());
    
    PRINT 'Cleaned up old sessions';
END

PRINT 'Database migration completed successfully!';
PRINT 'Remember to:';
PRINT '1. Update existing user passwords to use the new hashing system';
PRINT '2. Set up a scheduled job to run sp_CleanupOldSessions periodically';
PRINT '3. Monitor the security_audit table for suspicious activities';
PRINT '4. Encrypt the connection strings in web.config using aspnet_regiis.exe';