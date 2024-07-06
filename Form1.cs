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
                $"\r\nContent-Type: text/html;charset=utf-8\r\n";// +
                //$"\r\n<body>{text}</body>";

            var sr = new StringReader(body);
            List<string> listOfLines = new List<string>();
            string line;
            while ((line = sr.ReadLine()) != null)
            {// rn is for the formatting of the message <br> is line breaks
                if (line == "")
                {
                    Message = Message + "\r\n" + "<br>"; //if we need to skip an empty line, just add another break
                }
                else
                {
                    Message = Message + "\r\n" + "<br>" + line; //go to a new line before adding this one
                }
            }


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


        private void button1_Click(object sender, System.EventArgs e)
        {
            string credentialsFileLocation = "C:/Users/benja/source/repos/EmailTestProject/bin/Debug/credentials.json";
            string addressTo = txtTo.Text;
            string cc = txtCC.Text;
            string bcc = txtBcc.Text;
            string subject = txtSubject.Text;
            string body = txtMessage.Text;

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

    }
}
