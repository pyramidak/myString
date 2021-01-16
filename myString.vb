#Region " String "

Class myString

    ' Convert a string to a byte array
    Public Shared Function ToBytes(ByVal Text As String) As Byte()
        Dim encoding As New Text.ASCIIEncoding()
        Return encoding.GetBytes(Text)
    End Function

    ' Convert a byte array to a string:
    Public Shared Function FromBytes(ByVal arrBytes() As Byte) As String
        Dim Text As String = ""
        For i As Integer = LBound(arrBytes) To UBound(arrBytes)
            Text = Text & Chr(arrBytes(i))
        Next
        Return Text
    End Function

    Public Shared Function FromFile(filename As String) As String
        FromFile = ""
        If myFile.Exist(filename) = False Then Exit Function
        Try
            Dim tr As IO.TextReader = New IO.StreamReader(filename)
            FromFile = tr.ReadToEnd
            tr.Dispose()
        Catch
        End Try
    End Function

    Public Shared Function FromEmbeddedResource(filename As String) As String
        Dim stream As IO.Stream = Reflection.Assembly.GetExecutingAssembly.GetManifestResourceStream("RootSpace." & filename)
        Return FromStream(stream)
    End Function

    Public Shared Function FromStream(stream As IO.Stream, Optional size As Long = 0) As String
        stream.Seek(0, IO.SeekOrigin.Begin)
        Dim Reader As New IO.BinaryReader(stream)
        If size = 0 OrElse stream.Length < size Then size = stream.Length
        Dim fileByte() As Byte = Reader.ReadBytes(CInt(size))
        Reader.Close() : stream.Close()
        Return FromBytes(fileByte)
    End Function

    Public Shared Function FirstWord(ByVal Text As String) As String
        Dim iEndWord As Integer = Text.IndexOf(" ")
        If iEndWord = -1 Then
            Return Text
        Else
            Return Text.Substring(0, iEndWord)
        End If
    End Function

    Public Shared Function Left(ByVal Text As String, ByVal Length As Integer) As String
        If Text Is Nothing Then Return ""
        If Text.Length <= Length Then Return Text
        Text = Text.Substring(0, Length - 1)
        Dim cutLink As Integer = Strings.Left(Text, Length - 1).LastIndexOf(" ") + 1
        If cutLink < 2 Then Return Text
        Return Strings.Left(Text, cutLink)
    End Function

    Public Shared Function FromNumber(ByVal Number As Integer, ByVal NoNull As Integer) As String
        Return New String(CChar("0"), NoNull - Number.ToString.Length) + Number.ToString
    End Function

    Public Shared Function GetDouble(text As String) As Double
        If text Is Nothing OrElse text = "" Then Return -1
        Dim separator As String = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator
        If separator = "." Then
            text = text.Replace(",", ".")
        Else
            text = text.Replace(".", ",")
        End If

        If IsNumeric(text) Then Return CDbl(text)
        If IsNumeric(text.Substring(0, 1)) Then
            For a As Integer = 1 To text.Length
                If Not text.Substring(a, 1) = separator AndAlso IsNumeric(text.Substring(a, 1)) = False Then
                    Return CDbl(text.Substring(0, a))
                End If
            Next
        End If
        Return -1
    End Function

    Public Shared Function GetDate(text As String) As Date
        If text Is Nothing OrElse text = "" Then Return New Date
        'September 14, 2019 at 02:27PM
        If text.Length > 20 AndAlso text.Substring(text.Length - 10, 2) = "at" Then 'IFTTT date format
            text = text.Replace("at ", "")
            Try
                Return DateTime.ParseExact(text, "MMMM d, yyyy hh:mmtt", CultureInfo.InvariantCulture)
            Catch
                Return New Date
            End Try
        Else
            Try 'system date format
                Return DateTime.Parse(text, CultureInfo.CurrentCulture, DateTimeStyles.NoCurrentDateDefault)
            Catch ex As Exception
                Try
                    Return DateTime.Parse(text, New CultureInfo("en-US"), DateTimeStyles.NoCurrentDateDefault)
                Catch
                    Try
                        Return DateTime.Parse(text, New CultureInfo("zh-CN"), DateTimeStyles.NoCurrentDateDefault)
                    Catch
                        Try
                            Return DateTime.Parse(text, New CultureInfo("ru-RU"), DateTimeStyles.NoCurrentDateDefault)
                        Catch
                            Return New Date
                        End Try
                    End Try
                End Try
            End Try
        End If
    End Function

End Class

#End Region