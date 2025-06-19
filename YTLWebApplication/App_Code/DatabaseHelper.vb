Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Text.RegularExpressions

Public Class DatabaseHelper
    Private Shared ReadOnly ConnectionString As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    ' Execute parameterized query safely
    Public Shared Function ExecuteQuery(query As String, parameters As Dictionary(Of String, Object)) As DataTable
        Dim dataTable As New DataTable()
        
        Try
            Using connection As New SqlConnection(ConnectionString)
                Using command As New SqlCommand(query, connection)
                    ' Add parameters
                    If parameters IsNot Nothing Then
                        For Each param In parameters
                            command.Parameters.AddWithValue(param.Key, If(param.Value, DBNull.Value))
                        Next
                    End If
                    
                    connection.Open()
                    Using adapter As New SqlDataAdapter(command)
                        adapter.Fill(dataTable)
                    End Using
                End Using
            End Using
        Catch ex As Exception
            SecurityHelper.LogError("Database query failed", ex, Nothing)
            Throw New DatabaseException("Database operation failed", ex)
        End Try
        
        Return dataTable
    End Function

    ' Execute non-query with parameters
    Public Shared Function ExecuteNonQuery(query As String, parameters As Dictionary(Of String, Object)) As Integer
        Try
            Using connection As New SqlConnection(ConnectionString)
                Using command As New SqlCommand(query, connection)
                    ' Add parameters
                    If parameters IsNot Nothing Then
                        For Each param In parameters
                            command.Parameters.AddWithValue(param.Key, If(param.Value, DBNull.Value))
                        Next
                    End If
                    
                    connection.Open()
                    Return command.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            SecurityHelper.LogError("Database non-query failed", ex, Nothing)
            Throw New DatabaseException("Database operation failed", ex)
        End Try
    End Function

    ' Execute scalar query with parameters
    Public Shared Function ExecuteScalar(query As String, parameters As Dictionary(Of String, Object)) As Object
        Try
            Using connection As New SqlConnection(ConnectionString)
                Using command As New SqlCommand(query, connection)
                    ' Add parameters
                    If parameters IsNot Nothing Then
                        For Each param In parameters
                            command.Parameters.AddWithValue(param.Key, If(param.Value, DBNull.Value))
                        Next
                    End If
                    
                    connection.Open()
                    Return command.ExecuteScalar()
                End Using
            End Using
        Catch ex As Exception
            SecurityHelper.LogError("Database scalar query failed", ex, Nothing)
            Throw New DatabaseException("Database operation failed", ex)
        End Try
    End Function

    ' Validate SQL query to prevent injection
    Public Shared Function ValidateQuery(query As String) As Boolean
        ' Check for dangerous SQL keywords
        Dim dangerousPatterns() As String = {
            "xp_cmdshell", "sp_oacreate", "sp_oamethod", "sp_oagetproperty", "sp_oasetproperty",
            "openrowset", "opendatasource", "bulk insert", "exec\s*\(", "execute\s*\(",
            "union.*select", "insert.*into", "update.*set", "delete.*from", "drop\s+table",
            "create\s+table", "alter\s+table", "truncate\s+table"
        }

        For Each pattern In dangerousPatterns
            If Regex.IsMatch(query, pattern, RegexOptions.IgnoreCase) Then
                Return False
            End If
        Next

        Return True
    End Function

    ' Create secure connection with timeout
    Public Shared Function CreateSecureConnection() As SqlConnection
        Dim connectionStringBuilder As New SqlConnectionStringBuilder(ConnectionString) With {
            .ConnectTimeout = 30,
            .CommandTimeout = 30,
            .Encrypt = True,
            .TrustServerCertificate = False,
            .MultipleActiveResultSets = False
        }
        
        Return New SqlConnection(connectionStringBuilder.ConnectionString)
    End Function
End Class

Public Class DatabaseException
    Inherits Exception
    
    Public Sub New(message As String)
        MyBase.New(message)
    End Sub
    
    Public Sub New(message As String, innerException As Exception)
        MyBase.New(message, innerException)
    End Sub
End Class