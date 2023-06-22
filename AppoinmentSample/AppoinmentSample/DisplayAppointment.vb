Imports MySql.Data.MySqlClient

Public Class DisplayAppointment
    Public con As New MySqlConnection
    Private lisf1day As New List(Of FlowLayoutPanel)
    Private CurrentDate As DateTime = DateTime.Today
    Sub opencon()
        con.ConnectionString = "server=localhost;username=root;password=root;database=ams2"
        con.Open()

        ' Add code to populate the ComboBox with employee names
        'Dim query As String = "SELECT DISTINCT emp_name FROM employee"
        Dim query As String = "SELECT employee_id FROM employee_work_schedule where employee_id like @ID"
        Dim cmd As New MySqlCommand(query, con)
        cmd.Parameters.AddWithValue("@ID", "%" & ComboBox1.Text & "%")
        Dim reader As MySqlDataReader = cmd.ExecuteReader()
        Dim suggestions As New List(Of String)()
        While reader.Read()
            ComboBox1.Items.Add(reader("employee_id").ToString())
            suggestions.Add(reader("employee_id").ToString())
        End While
        reader.Close()

        ' Enable autocomplete feature
        ComboBox1.AutoCompleteMode = AutoCompleteMode.Suggest
        ComboBox1.AutoCompleteSource = AutoCompleteSource.CustomSource
        ComboBox1.AutoCompleteCustomSource.AddRange(suggestions.ToArray())
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged
        displaycurrentdate()
    End Sub


    'Private Function GetEmployeeSchedule(ByVal employeeName As String) As List(Of String)
    '    Dim scheduleList As New List(Of String)

    '    Dim firstDayOfMonth As DateTime = New DateTime(CurrentDate.Year, CurrentDate.Month, 1)
    '    Dim lastDayOfMonth As DateTime = firstDayOfMonth.AddMonths(1).AddDays(-1)

    '    Dim query As String = "SELECT TIME(shift_start) AS shift_start, TIME(shift_end) AS shift_end FROM employee WHERE emp_name = @emp_name"
    '    Dim cmd As New MySqlCommand(query, con)
    '    cmd.Parameters.AddWithValue("@emp_name", employeeName)
    '    cmd.Parameters.AddWithValue("@start_date", firstDayOfMonth.ToString("yyyy-MM-dd"))
    '    cmd.Parameters.AddWithValue("@end_date", lastDayOfMonth.ToString("yyyy-MM-dd"))
    '    Return scheduleList
    'End Function

    Private Sub DisplayAppointment_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        opencon()
        GenerateDayPanel(42)
        displaycurrentdate()
    End Sub

    Private Function gettotaldaysofcurrentdate() As Integer
        Dim firstdayofcurrentdate As DateTime = New Date(CurrentDate.Year, CurrentDate.Month, 1)
        Return firstdayofcurrentdate.AddMonths(1).AddDays(-1).Day
    End Function

    Private Function getfirstdayofweekofcurrentdate() As Integer
        Dim firstdayofmonth As DateTime = New Date(CurrentDate.Year, CurrentDate.Month, 1)
        Return firstdayofmonth.DayOfWeek + 1
    End Function

    Private Sub displaycurrentdate()
        lblmonthyear.Text = CurrentDate.ToString("MMMM, yyyy")
        Dim firstday As Integer = getfirstdayofweekofcurrentdate()
        Dim totalday As Integer = gettotaldaysofcurrentdate()
        addlabel(firstday, totalday)
        AddAppointmenttoF1Day(firstday)
    End Sub

    Private Sub prevmonth()
        CurrentDate = CurrentDate.AddMonths(-1)
        displaycurrentdate()
    End Sub

    Private Sub nextmonth()
        CurrentDate = CurrentDate.AddMonths(1)
        displaycurrentdate()
    End Sub

    Private Sub today1()
        CurrentDate = DateTime.Today
        displaycurrentdate()
    End Sub

    Private Sub GenerateDayPanel(ByVal totaldays As Integer)
        FlowLayoutPanel1.Controls.Clear()
        lisf1day.Clear()
        Dim days As Integer = DateTime.DaysInMonth(Now.Year, Now.Month)
        For i As Integer = 1 To totaldays
            Dim f1 As New FlowLayoutPanel
            f1.Size = New Size(116, 102)
            f1.BorderStyle = BorderStyle.FixedSingle
            FlowLayoutPanel1.Controls.Add(f1)
            lisf1day.Add(f1)
        Next
    End Sub


    'add label to date 1-30
    Private Sub addlabel(ByVal startday As Integer, ByVal totaldaysinmonth As Integer)
        For Each f1 As FlowLayoutPanel In lisf1day
            f1.Controls.Clear()
        Next

        For i As Integer = 1 To totaldaysinmonth
            Dim lbl As New Label()
            lbl.Name = "lblDay" & i
            lbl.AutoSize = False
            lbl.TextAlign = ContentAlignment.TopCenter
            lbl.Size = New Size(112, 45)
            lbl.Text = i.ToString()
            lbl.Font = New Font("Segoe UI", 24)

            Dim index As Integer = (i - 1) + (startday - 1)
            If index >= 0 AndAlso index < lisf1day.Count Then
                lisf1day(index).Controls.Clear()
                lisf1day(index).Controls.Add(lbl)
            End If
        Next
    End Sub

    Private Sub btnprev_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnprev.Click
        prevmonth()
    End Sub

    Private Sub btnnext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnnext.Click
        nextmonth()
    End Sub

    Private Sub btntoday_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btntoday.Click
        today1()
        GenerateDayPanel(42)
        displaycurrentdate()
    End Sub

    Public Function QueryAsDataTable(ByVal sql As String) As DataTable
        Dim con As New MySqlConnection("server=localhost;username=root;password=root;database=ams2")
        Dim da As New MySqlDataAdapter()
        da.SelectCommand = New MySqlCommand(sql, con)
        Dim dt As New DataTable()
        da.Fill(dt)
        Return dt
    End Function


    Private Sub AddAppointmenttoF1Day(ByVal startatF1Number As Integer)
        Dim startDate As DateTime = New Date(CurrentDate.Year, CurrentDate.Month, 1)
        Dim endDate As DateTime = startDate.AddMonths(1).AddDays(-1)
        'Dim sql As String = "SELECT TIME(date_from) AS shift_start, TIME(date_to) AS shift_end, TIME(break_from) AS break_start, TIME(break_to) AS break_end FROM employee_work_schedule WHERE employee_id = '" & ComboBox1.Text & "' limit 1"
        'Dim sql As String = "SELECT TIME(date_from) AS shift_start, TIME(date_to) AS shift_end, TIME(break_from) AS break_start, TIME(break_to) AS break_end FROM employee_work_schedule WHERE employee_id = '" & ComboBox1.Text & "' AND date_from BETWEEN '" & startDate.ToString("yyyy-MM-dd") & "' AND '" & endDate.ToString("yyyy-MM-dd") & "' limit 1"
        Dim sql As String = "SELECT TIME(date_from) AS shift_start, TIME(date_to) AS shift_end, TIME(break_from) AS break_start, TIME(break_to) AS break_end FROM employee_work_schedule WHERE employee_id = '" & ComboBox1.Text & "' AND date_from BETWEEN '" & startDate.ToString("yyyy-MM-dd") & "' AND '" & endDate.ToString("yyyy-MM-dd") & "' limit 1"
        Dim queryString As String = "SELECT DATE_FORMAT(date_from, '%y-%m-%d') AS schedule FROM employee_work_schedule WHERE date_from >= '" & startDate.ToString("yyyy-MM-dd") & "' AND date_from <= '" & endDate.ToString("yyyy-MM-dd") & "' AND employee_id = '" & ComboBox1.Text & "' LIMIT 1"

        Dim schedule As String = "" ' Declare and initialize the 'schedule' variable

        Using command As MySqlCommand = New MySqlCommand(queryString, con)
            schedule = Convert.ToString(command.ExecuteScalar()) ' Assign a value to the 'schedule' variable
        End Using

        Dim dt As DataTable = QueryAsDataTable(sql)
        Dim firstdayofmonth As DateTime = New Date(CurrentDate.Year, CurrentDate.Month, 1)
        Dim currentDay As DateTime = firstdayofmonth.AddDays(1 - getfirstdayofweekofcurrentdate())
        Dim totalDaysInMonth As Integer = gettotaldaysofcurrentdate()

        For Each row As DataRow In dt.Rows
            Dim shiftStart As TimeSpan = TimeSpan.Parse(row("shift_start").ToString())
            Dim shiftEnd As TimeSpan = TimeSpan.Parse(row("shift_end").ToString())
            Dim breakStart As TimeSpan = TimeSpan.Parse(row("break_start").ToString())
            Dim breakEnd As TimeSpan = TimeSpan.Parse(row("break_end").ToString())
            Dim appDay As DateTime = DateTime.MinValue.Add(shiftStart)
            Dim appDay1 As DateTime = DateTime.MinValue.Add(shiftEnd)
            Dim appDaystart As DateTime = DateTime.MinValue.Add(breakStart)
            Dim appDay1end As DateTime = DateTime.MinValue.Add(breakEnd)
            'Dim dateFrom As DateTime = DateTime.Parse(row("date_from")) ' Extract the date_from value from the row
            For day As Integer = 1 To totalDaysInMonth
                Dim currentDate As DateTime = firstdayofmonth.AddDays(day - 1)

                'If currentDate >= startDate AndAlso currentDate <= endDate AndAlso currentDate.DayOfWeek <> DayOfWeek.Sunday Then
                'If currentDate.Date >= formattedSchedule Then
                If currentDate.Date >= DateTime.ParseExact(schedule, "yy-MM-dd", Nothing) Then

                    ' MsgBox(currentDate)
                    ' Display appointment label
                    Dim label As New Label()
                    label.Name = "lblAppointment"
                    label.AutoSize = False
                    label.TextAlign = ContentAlignment.BottomCenter
                    label.Size = New Size(115, 40)
                    label.Font = New Font("Segoe UI", 9)
                    label.Text = appDay.ToString("hh\:mm tt") & " - " & appDay1.ToString("hh\:mm tt") & Environment.NewLine & appDaystart.ToString("hh\:mm tt") & " - " & appDay1end.ToString("hh\:mm tt")

                    ' Add the appointment label to the corresponding panel
                    Dim panelIndex As Integer = currentDate.Day + (startatF1Number - 2)
                    lisf1day(panelIndex).Controls.Add(label)

                ElseIf currentDate.DayOfWeek = DayOfWeek.Sunday Then
                    ' Display rest day label
                    Dim restLabel As New Label()
                    restLabel.Name = "lblRestDay"
                    restLabel.AutoSize = False
                    restLabel.TextAlign = ContentAlignment.MiddleCenter
                    restLabel.Size = New Size(111, 15)
                    restLabel.ForeColor = Color.White
                    restLabel.BackColor = Color.Red
                    restLabel.Font = New Font("Segoe UI", 9)

                    restLabel.Text = "Rest Day"

                    ' Add the rest day label to the corresponding panel
                    Dim panelIndex As Integer = currentDate.Day + (startatF1Number - 2)
                    lisf1day(panelIndex).Controls.Add(restLabel)
                ElseIf currentDay.Day = DayOfWeek.Sunday Then
                    '
                    MsgBox("check")
                End If

            Next
        Next
    End Sub
End Class