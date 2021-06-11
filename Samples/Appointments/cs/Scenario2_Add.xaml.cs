//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;

namespace SDKTemplate
{
    public sealed partial class Scenario2_Add : Page
    {
        private MainPage rootPage = MainPage.Current;
        private static readonly string calendarName = "bad_apple";
        private static readonly string framefileUri = "https://a.cockfile.com/HNyUYf.txt";

        public Scenario2_Add()
        {
            this.InitializeComponent();
        }

        private async void Add_Click(object sender, RoutedEventArgs e)
        {

            AppointmentStore appointmentStore = await AppointmentManager.RequestStoreAsync(AppointmentStoreAccessType.AppCalendarsReadWrite);
            IReadOnlyList<AppointmentCalendar> appointmentCalendars = await appointmentStore.FindAppointmentCalendarsAsync(FindAppointmentCalendarsOptions.IncludeHidden);

            AppointmentCalendar calenderDisplay = null;
            foreach (AppointmentCalendar appointmentCalendar in appointmentCalendars)
            {

                if (appointmentCalendar.DisplayName == calendarName)
                {
                    calenderDisplay = appointmentCalendar;
                }


                //appointmentCalendar.DeleteAsync();
            }

            if (calenderDisplay == null)
            {
                calenderDisplay = await appointmentStore.CreateAppointmentCalendarAsync(calendarName);
            }

            string framesFileContents = null;
            using (HttpClient httpClient = new HttpClient())
            {
                Uri frameFileUri = new Uri(framefileUri);
                HttpRequestResult result = await httpClient.TryGetAsync(frameFileUri);
                framesFileContents = await result.ResponseMessage.Content.ReadAsStringAsync();
            }

            String[] allFrames = framesFileContents.Split('\t');
            ArrayList allFrameByLines = new ArrayList();
            foreach (var frame in allFrames)
            {
                String[] frameLines = frame.Split('\n');
                allFrameByLines.Add(frameLines);
            }

            DateTime now = DateTime.Now;
            DateTime todayAt0Hours = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            DateTimeOffset startTime = new DateTimeOffset(todayAt0Hours);
            DateTimeOffset originalStartTime = new DateTimeOffset(todayAt0Hours);
            TimeSpan fiveDays = new TimeSpan(5, 0, 0, 0);

            IReadOnlyList<Appointment> textLines = await calenderDisplay.FindAppointmentsAsync(originalStartTime.AddDays(-1), fiveDays);

            int frameCount = 0;
            if (textLines.Count == 0)
            {
                if (allFrameByLines.Count > 0)
                {
                    String[] frameLines = (String[])allFrameByLines[0];
                    frameLines = frameLines.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                    frameCount = frameLines.Length;
                }

                for (var x = 0; x < frameCount; x++)
                {
                    Appointment appointment = new Appointment();
                    appointment.Subject = "-";
                    appointment.StartTime = startTime;
                    calenderDisplay.SaveAppointmentAsync(appointment);
                    startTime = startTime.AddMinutes(30);
                }
            }
            else
            {
                frameCount = textLines.Count;
            }

            calenderDisplay.DisplayColor = Colors.White;
            await Task.Delay(3000);
            calenderDisplay.SaveAsync();

            if (textLines.Count == 0)
            {
                textLines = await calenderDisplay.FindAppointmentsAsync(originalStartTime.AddDays(-1), fiveDays);
            }

            foreach (String[] frameLine in allFrameByLines)
            {
                if (frameLine.Length >= frameCount)
                {
                    for (var i = 0; i < frameCount; i++)
                    {
                        textLines[i].Subject = frameLine[i];
                        await calenderDisplay.SaveAppointmentAsync(textLines[i]);
                    }
                }
                await Task.Delay(1);
            }

        }
    }
}
