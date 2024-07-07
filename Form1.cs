using System.Windows.Forms;
using System.Text;
using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

using Google.Apis.Gmail.v1;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Net.Mail;

//uses gmail API instead of smtp as google got rid of "let less secure apps access" feature in favour of API 0-Auth2
//requires Gmail API package is installed (do via tools and nuget)

namespace EmailTestProject
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public void SendEmail(string credentialsFileLocation, string addressTo, string body = "", string subject = "", string cc = "", string bcc = "")
        {   
            // variable = "" means that these parameters are optional, if nothing is passed for them, the default will be used (in this case the default is "")
            // reason for the wierd ordering is so that optional parameters come up in the order of their probabilty of use

            //see below for optional arguments
            //https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/named-and-optional-arguments#:~:text=productName%7D%22)%3B%0A%20%20%20%20%7D%0A%7D-,Optional%20arguments,-The%20definition%20of
            //or
            //https://www.w3schools.com/cs/cs_method_parameters_default.phpS
            //

            string[] Scopes = { GmailService.Scope.GmailSend };
            string ApplicationName = "SendMail";

            string Base64UrlEncode(string input)
            {
                var data = Encoding.UTF8.GetBytes(input);
                return Convert.ToBase64String(data).Replace("+", "-").Replace("/", "_").Replace("=", "");
            }

            UserCredential credential;
            //read your credentials file
            using (FileStream stream = new FileStream(credentialsFileLocation, FileMode.Open, FileAccess.Read))
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                path = Path.Combine(path, ".credentials/gmail-dotnet-quickstart.json");
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets, Scopes, "user", CancellationToken.None, new FileDataStore(path, true)).Result;
            }
            //compile the email
            string Message = $"To:{addressTo}" +
                $"\r\nCC:{cc}" +
                $"\r\nBcc:{bcc}" +
                $"\r\nSubject:{subject}" +
                $"\r\nContent-Type: text/html;charset=utf-8\r\n" +
                $"\r\n<body>{body}</body>";

            //call your gmail service, giving your credentials
            var service = new GmailService(new BaseClientService.Initializer() { HttpClientInitializer = credential, ApplicationName = ApplicationName });
            var msg = new Google.Apis.Gmail.v1.Data.Message();
            msg.Raw = Base64UrlEncode(Message.ToString());
            try
            {
                service.Users.Messages.Send(msg, "me").Execute();
                MessageBox.Show("Your email has been successfully sent !", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception)
            {
                MessageBox.Show("There was an error in sending your email", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        //below is for a computer email - should be written in html
        static string roomType = "Gold ensuite family";
        static Random rand = new Random(10);
        static int adults = rand.Next(1, 2);
        static int kids = rand.Next(0, 2);
        static string checkInDate = "11/09/24";
        static string checkOutDate = "15/09/24";
        static string bookingReference = "DQ54Y7";
        static string bookingName = "Benjamin Franklin";
        static string bookingEmail = "benjaminjfranklin99@gmail.com";

        string computerbody = "" +
            "<style>" +
            "h1 {text-align: center;}" +
            "body {text-align: center;}" +
            "</style>" +
            "" +
            "<img src=\"https://www.weather2travel.com/images_blog/butlins-discount-code-holiday-parks-offers.png\" " +
            "alt=\"Butlin's Venues\" class = \"centre\", width=\"780\" height=\"190\">" +
            "<body>" +
            "<h1>Your great stay awaits!</h1>" +
            "<p>Thank you for booking your break with Butlin's! " +
            "The details of your stay can be found below</p>" +
            $"<p>Dates: {checkInDate} - {checkOutDate}" +
            $"<br /> Room Type: {roomType} " +
            "<br /> " +
            $"<br /> Number of guests: " +
            $"<br /> Adults: {adults} " +
            $"<br /> Kids: {kids}" +
            "</p><hr><P>" +
            "<centre>Your Booking Reference</centre>" +
            $"<br /><h1>{bookingReference}</h1>" +
            $"<br />Name: {bookingName}" +
            $"<br />Booking Email: {bookingEmail}" +
            "</p><hr><P>" +
            "<h1>Checking in</h1>" +
            "Here’s all you need to know to make sure you have a speedy and smooth arrival and check-in experience." +
            "<ul>" +
            "<li>You’ll receive an e-mail from 6pm the day before you arrive to confirm your arrival information, including your allocated arrival entrance" +
            "<li>When you get to Butlin’s, you’ll need to head to your allocated arrival entrance. It’s really important that you arrive at the right arrival entrance to ensure a safe and easy check-in" +
            "<li>Our team will direct you onto our drive through check-in, where your welcome pack and key card will be waiting for you. All you need is your booking reference number and photo ID ready to show our team" +
            "<li>Once you’re checked in, you’ll be directed to your accommodation and the nearest car park. Don’t worry, there are lots of free on-resort parking spaces" +
            "<li>Once you’ve parked up, there are plenty of luggage trolleys to help you unload the car and get your belongings into your accommodation. Trolley parks are located in or near the car park." +
            "</ul>" +
            "</p><hr><P>" +
            "<h1>How to find us</h1>" +
            "Finding us is easy! Our address is:" +
            "<br />Butlin’s Minehead,<br />Warren Road,<br />Minehead,<br />Somerset,<br />TA24 5SH" +
            "<br /><br /><a href=\"https://www.google.co.uk/maps/place/Butlin's+Minehead+Resort/@51.2066906,-3.4614799,17z/data=!3m1!4b1!4m8!3m7!1s0x486de75601c5033b:0xdf889f0748035f07!5m2!4m1!1i2!8m2!3d51.2066906!4d-3.4614799\">Find us on Google Maps</a>" +
            "<br /><br />General enquiries: 0330 100 6649<br />Special requirements: 0330 100 9334 (select option 2)" +
            "<br /><br />ARRIVING VIA PUBLIC TRANSPORT?\r\n\r\nIf you're arriving by public transport, you'll need to head to the West Entrance, " +
            "where our team will be ready to direct to your check-in location." +
            "<br /><br />From Minehead Station, the resort is only a 10 minute walk, with scenic views along the way. " +
            "<a href=\"https://maps.app.goo.gl/RjwU3fuhUeZFsGhV9\">Click here for directions.</a>" +
            "<img src=\"https://prod.butlins-prod.magnolia-platform.com/.imaging/focalpoint/landscape2-1/1000x/dam/jcr:1f134d14-088d-4eba-a0f3-46fbbba5cf0f/Family-Images/Resorts/Butlins-Resorts-Map-750px.jpg\" " +
            "alt=\"Uk Map of Butlins Sites\" class = \"centre\">" +
            "</p><hr><P>" +
            "If you need any more assistance please visit our website <a href=\"https://www.butlins.com/\">www.butlins.com</a>" +
            $"<br /> We can't wait to see you!" +
            $"" +
            $"</body>";

        private void button1_Click(object sender, System.EventArgs e)
        {
            string credentialsFileLocation = "C:/Users/benja/source/repos/EmailTestProject/bin/Debug/credentials.json";
            string addressTo = txtTo.Text;
            string cc = txtCC.Text;
            string bcc = txtBcc.Text;
            string subject = txtSubject.Text;
            //string body = convertMultilineStringToHTML(txtMessage.Text);
            string body = computerbody; //if want to send the computer email with the button

            //for info on how to send parameters out of order see this
            //https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/named-and-optional-arguments#:~:text=If%20you%20don%27t%20remember%20the%20order%20of%20the%20parameters%20but%20know%20their%20names%2C%20you%20can%20send%20the%20arguments%20in%20any%20order.
            SendEmail(credentialsFileLocation: credentialsFileLocation, addressTo: addressTo, body: body, subject: subject, cc: cc, bcc: bcc);

            //for this form the above is best as we dont mind sending empty text anyways but if the computer was writing the email
            //the below options may serve better

            //equally, you don't need to send all/any of the optional parametrs
            //sendEmail(credentialsFileLocation: credentialsFileLocation, addressTo: addressTo);

            //or you can send the parameters out of order so long as you specifiy what parameters you are sending
            //(credentialsFileLocation: credentialsFileLocation, addressTo: addressTo, body: body, subject: subject, cc: cc, bcc: bcc);

            //or you can just send them normally, in order
            //sendEmail(credentialsFileLocation, addressTo, body, subject, cc, bcc);

        }
        private string convertMultilineStringToHTML(string text)
        {
            string formattedString = "";
            //split windows forms input into multiline html format for email
            var sr = new StringReader(text);
            List<string> listOfLines = new List<string>();
            string line;
            bool firstLine = true;
            while ((line = sr.ReadLine()) != null)
            {// rn is for the formatting of the message <br> is line breaks
                if (line == "")
                {
                    formattedString = formattedString + "\r\n" + "<br>"; //if we need to skip an empty line, just add another break
                }
                else
                {
                    if (firstLine)
                    {
                        formattedString = formattedString + "\r\n" + line; //dont need a new line initially
                    }
                    else
                    {
                        formattedString = formattedString + "\r\n" + "<br>" + line; //go to a new line before adding this one
                    }
                }
                firstLine = false;
            }
            return formattedString;
        }

    }
}
