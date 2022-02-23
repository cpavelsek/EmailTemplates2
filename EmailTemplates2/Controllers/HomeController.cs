using EmailTemplates2.Models;
//using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net.Mail;
using static EmailTemplates2.Startup;
using System.Text;
using Passbook.Generator;
using System.Security.Cryptography.X509Certificates;
using Passbook.Generator.Fields;
using System.Security.Cryptography;
using System.IO;
using Google_Pay_ClassLibrary;

namespace EmailTemplates2.Controllers {
	public class HomeController : Controller {
		private readonly ILogger<HomeController> _logger;

		public HomeController( ILogger<HomeController> logger ) {
			_logger = logger;
		}

		public IActionResult Index() {
			return View();
		}

		public IActionResult Privacy() {
			return View();
		}

		[ResponseCache( Duration = 0, Location = ResponseCacheLocation.None, NoStore = true )]
		public IActionResult Error() {
			return View( new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier } );
		}

        public IActionResult Contact() {
            ViewData[ "Message" ] = "Your contact page.";

            return View();
        }

        //[HttpPost]
        //public IActionResult Contact( ContactViewModel contactViewModel ) {
        //    if ( ModelState.IsValid ) {
        //        try {
        //            string emailBody = string.Empty;
        //            bool useEmailTemplate = true;
        //            //instantiate a new MimeMessage
        //            var message = new MimeMessage();

        //            //Setting the To e-mail address
        //            message.To.Add( new MailboxAddress( contactViewModel.Name, contactViewModel.Email ) );
        //            //Setting the From e-mail address
        //            message.From.Add( new MailboxAddress( "Christopher", "thebigdog905@gmail.com" ) );
        //            //E-mail subject 
        //            message.Subject = contactViewModel.Subject;
        //            //E-mail message body
        //            if ( useEmailTemplate ) {
        //                emailBody = ViewsToStringOutputHelper.RenderRazorViewToString( this, "Welcome", contactViewModel );
        //            } else {
        //                emailBody = contactViewModel.Message + " Message was sent by: " + contactViewModel.Name + " E-mail: " + contactViewModel.Email;

        //            }
        //            message.Body = new TextPart( TextFormat.Html ) {
        //                Text = emailBody
        //            };

        //            //Configure the e-mail
        //            using ( var emailClient = new SmtpClient() ) {
        //                emailClient.Connect( "smtp.gmail.com", 587, false );
        //                emailClient.Authenticate( "thebigdog905@gmail.com", "sparten117" );
        //                emailClient.Send( message );
        //                emailClient.Disconnect( true );
        //            }
        //        }
        //        catch ( Exception ex ) {

        //            ModelState.Clear();
        //            ViewBag.Exception = $" Oops! Message could not be sent. Error:  {ex.Message}";
        //        }

        //    }
        //    return View("Index");
        //}

        [HttpPost]
        public IActionResult Contact(ContactViewModel contactViewModel)
        {

            //grab pass from AppleWallet method.  comes in Byte format so that it can be translated to "Attachment"
            var pass = ContactIPhone();

           // var attachment = new Attachment(pass, mediaType);
            //try to get code/link/button sent over to email
            //var loyaltyClass = "https://walletobjects.googleapis.com/walletobjects/v1/loyaltyClass";

            string to = "thebigdog905@gmail.com"; //To address    
            //string to = "travisj@cidesigninc.com"; //To address  
            string from = contactViewModel.Email; //From address    
            MailMessage message = new MailMessage(from, to);

           // "<a href=''> <img src='~/Themes/Notabene/img/notabene/NoBtn.png' width=50 height=31 border=0 alt=Click title=Click ></a> ";
            message.Subject = contactViewModel.Subject;
            message.Body = contactViewModel.Message += "<a href='https://pay.google.com/gp/v/save/eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJhdWQiOiJnb29nbGUiLCJ0eXAiOiJzYXZldG9hbmRyb2lkcGF5IiwiaXNzIjoiY2lkLXFyLXRlc3RpbmdAY2lkLXFyLXRlc3RpbmcuaWFtLmdzZXJ2aWNlYWNjb3VudC5jb20iLCJvcmlnaW5zIjpbImh0dHA6Ly9sb2NhbGhvc3Q6ODA4MCJdLCJpYXQiOjE2NDQ0NDI5MjEsInBheWxvYWQiOnsiZXZlbnRUaWNrZXRPYmplY3RzIjpbeyJpZCI6IjMzODgwMDAwMDAwMjIwNzI3MTAuRVZFTlRUSUNLRVRfT0JKRUNUX2YwYjA5NzQ0LTNmZjEtNGM4Yy04ZWE4LTA3MDkwYTYwYmNiYyJ9XX19.gPrke5Syhgb_5ARqZhhNyUEmdpxu3e0SbpFXl7x4fp7EZvf9I6vmf0Wt6p73_cAZz8SByemSY5sXeETz8hK1BSKe3mokHY4QDngYT1pgkkeMvxofJDCO9HEpdi4y2UekCTvX3SAOyoCTfATsDO6I6yO08bBYWJUPvms1mnvSjn27sp1YPwnRj4ZbExtJvVNVmAJ5jodTzGgykCKpxZ1xyaU8EckrP-1Y0dMv-xMHmRXaIC4Nssvy3G76iC41g8tIU9xxczj7CkHsRxaFW2XSkxOVb28OfnUDPPJQZC8sM65mqBCc2AqPUlYkBWep5B4SHMe47AjNTj1HR3sZltHCeQ?hsas=1&pli=1' class='btn'> Click Me </a>";
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;
            message.Attachments.Add(new Attachment(new MemoryStream(pass), "test.pkpass"));
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587); //Gmail smtp    
            System.Net.NetworkCredential basicCredential1 = new System.Net.NetworkCredential("thebigdog905@gmail.com", "sparten117");
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = basicCredential1;
            try
            {
                client.Send(message);
            }

            catch (Exception ex)
            {
                throw ex;
            }
            return View("Index");
        }

        
        public Byte[] ContactIPhone()
        {
            PassGenerator generator = new PassGenerator();

            PassGeneratorRequest request = new PassGeneratorRequest();

            request.PassTypeIdentifier = "pass.apple-wallet-testing";
            request.TeamIdentifier = "24953BQKAA";
            request.SerialNumber = "7e9d38d99ffa3e442c2d66768ad8f947";
            request.Description = "BigDog Social Mixer";
            request.OrganizationName = "CI Design";
            request.LogoText = "2022 BigDog Social Mixer";


            //styling
            request.BackgroundColor = "#FFFFFF";
            request.LabelColor = "#000000";
            request.ForegroundColor = "#000000";
            request.BackgroundColor = "rgb(255,255,255)";
            request.LabelColor = "rgb(0,0,0)";
            request.ForegroundColor = "rgb(0,0,0)";

            //WorldWide Developer Relations
            request.AppleWWDRCACertificate = new X509Certificate2("D:/SRC/Clients/AppleWallet Testing/AppleWWDRCA.cer");
            request.PassbookCertificate = new X509Certificate2("D:/SRC/Clients/AppleWallet Testing/passbook.pfx", "sparten");
    
            //Images
            request.Images.Add(PassbookImage.Icon, System.IO.File.ReadAllBytes("C:/Users/chris/Downloads/frother.jpg"));
            request.Images.Add(PassbookImage.Icon2X, System.IO.File.ReadAllBytes("C:/Users/chris/Downloads/frother.jpg"));
            request.Images.Add(PassbookImage.Icon3X, System.IO.File.ReadAllBytes("C:/Users/chris/Downloads/frother.jpg"));

            //customize style
            request.Style = PassStyle.EventTicket;

            request.AddPrimaryField(new StandardField("origin", "Location", "Milwaukee"));
            //request.AddPrimaryField(new StandardField("destination", "London", "LDN"));

            request.AddSecondaryField(new StandardField("boarding-gate", "VIP", "No"));

            request.AddAuxiliaryField(new StandardField("seat", "Time", "3:00 p.m."));
            request.AddAuxiliaryField(new StandardField("passenger-name", "Guest", "Kevin B"));

            request.TransitType = TransitType.PKTransitTypeGeneric;

            //BarCode/QR Code
            request.AddBarcode(BarcodeType.PKBarcodeFormatPDF417, "01927847623423234234", "ISO-8859-1", "01927847623423234234");

            //Package up all of the request.
            byte[] generatedPass = generator.Generate(request);


            //System.Net.Mail.Attachment pass = new System.Net.Mail.Attachment(generatedPass.ToString());

            //the two lines below will create a .pkpass using FileContent
            //return new FileContentResult(generatedPass, "application/vnd.apple.pkpass");
            //return File(generatedPass, "application/vnd.apple.pkpass", "Apple-Wallet-Pass.pkpass");

            //in order to try and send as an email attachment - we need to leave the pass in Byte format and convert in the email method
            return (generatedPass);


        }

        public void googlePass()
        {
            Config config = Config.getInstance();
            string choice = "z";
            string choices = "beglotq";
            Services.VerticalType verticalType = Services.VerticalType.OFFER;
            while (!choices.Contains(choice))
            {
                System.Console.WriteLine("\n\n*****************************\n" +
                        "Which pass type would you like to demo?\n" +
                        "b - Boarding Pass\n" +
                        "e - Event Ticket\n" +
                        "g - Gift Card\n" +
                        "l - Loyalty\n" +
                        "o - Offer\n" +
                        "t - Transit\n" +
                        "q - Quit\n" +
                        "\n\nEnter your choice:");
                choice = Console.ReadLine();
                switch (choice)
                {
                    case "b":
                        verticalType = Services.VerticalType.FLIGHT;
                        break;
                    case "e":
                        verticalType = Services.VerticalType.EVENTTICKET;
                        break;
                    case "g":
                        verticalType = Services.VerticalType.GIFTCARD;
                        break;
                    case "l":
                        verticalType = Services.VerticalType.LOYALTY;
                        break;
                    case "o":
                        verticalType = Services.VerticalType.OFFER;
                        break;
                    case "t":
                        verticalType = Services.VerticalType.TRANSIT;
                        break;
                    case "q":
                        return;
                    default:
                        System.Console.WriteLine("\n* Invalid choice. Please select one of the pass types by entering it''s related letter.\n");
                        break;
                }
            }

            // your classUid should be a hash based off of pass metadata, for the demo we will use pass-type_class_uniqueid
            string UUID = Guid.NewGuid().ToString();
            string classUid = $"{verticalType.ToString()}_CLASS_{UUID}";

            // check Reference API for format of "id", for example offer: (https://developers.google.com/pay/passes/reference/v1/offerclass/insert).
            // must be alphanumeric characters, ".", "_", or "-".
            string classId = $"{config.getIssuerId()}.{classUid}";

            // your objectUid should be a hash based off of pass metadata, for the demo we will use pass-type_object_uniqueid
            string UUIDobj = Guid.NewGuid().ToString();
            string objectUid = $"{verticalType.ToString()}_OBJECT_{UUIDobj}";

            // check Reference API for format of "id", for example offer:(https://developers.google.com/pay/passes/reference/v1/offerobject/insert).
            // Must be alphanumeric characters, ".", "_", or "-".
            string objectId = $"{config.getIssuerId()}.{objectUid}";

            // demonstrate the different "services" that make links/values for frontend to render a functional "save to phone" button
            demoFatJwt(verticalType, classId, objectId);
            demoObjectJwt(verticalType, classId, objectId);
            demoSkinnyJwt(verticalType, classId, objectId);

            // Get the specific Offer Object
            //OfferObject obj = client.offerobject().get("2945482443380251551.ExampleObject1").execute();
            //// Update the version and state
            //obj.setVersion(obj.getVersion() + 1L);
            //obj.setState("expired"); //see the Reference API for valid "state" options
            //// Update the Offer Object
            //OfferObject returnObj = client.offerobject().update(obj.getId(), obj).execute();
        }

        private static String SAVE_LINK = "https://pay.google.com/gp/v/save/"; // Save link that uses JWT. See https://developers.google.com/pay/passes/guides/get-started/implementing-the-api/save-to-google-pay#add-link-to-email
        public static void demoFatJwt(Services.VerticalType verticalType, String classId, String objectId)
        {
            System.Console.WriteLine(
                   "#############################\n" +
                   "#\n" +
                   "#  Generates a signed \"fat\" JWT.\n" +
                   "#  No REST calls made.\n" +
                   "#\n" +
                   "#  Use fat JWT in JS web button.\n" +
                   "#  Fat JWT is too long to be used in Android intents.\n" +
                   "#  Possibly might break in redirects.\n" +
                   "#\n" +
                   "#############################\n"
               );

            Services services = new Services();
            String fatJwt = services.makeFatJwt(verticalType, classId, objectId);

            if (fatJwt != null)
            {
                System.Console.WriteLine($"This is a \"fat\" jwt:\n{fatJwt}\n");
                System.Console.WriteLine("you can decode it with a tool to see the unsigned JWT representation:\nhttps://jwt.io\n");
                System.Console.WriteLine($"Try this save link in your browser:\n{SAVE_LINK}{fatJwt}\n");
                System.Console.WriteLine($"However, because a \"fat\" jwt is long, they are not suited for hyperlinks (get truncated). Recommend only using \"fat\" JWt with web-JS button only. Check:\nhttps://developers.google.com/pay/passes/reference/s2w-reference");
            }
            return;
        }
        public static void demoObjectJwt(Services.VerticalType verticalType, String classId, String objectId)
        {
            System.Console.WriteLine(
                "#############################\n" +
                "#\n" +
                "#  Generates a signed \"object\" JWT.\n" +
                "#  1 REST call is made to pre-insert class.\n" +
                "#\n" +
                "#  This JWT can be used in JS web button.\n" +
                "#  If this JWT only contains 1 object, usually isn't too long; can be used in Android intents/redirects.\n" +
                "#\n" +
                "#############################\n"
            );

            Services services = new Services();
            String objectJwt = services.makeObjectJwt(verticalType, classId, objectId);

            if (objectJwt != null)
            {
                System.Console.WriteLine($"This is an \"object\" jwt:\n{objectJwt}\n");
                System.Console.WriteLine($"you can decode it with a tool to see the unsigned JWT representation:\nhttps://jwt.io\n");
                System.Console.WriteLine($"Try this save link in your browser:\n{SAVE_LINK}{objectJwt}\n");
            }
            return;
        }

        public static void demoSkinnyJwt(Services.VerticalType verticalType, String classId, String objectId)
        {
            System.Console.WriteLine(
                "#############################\n" +
                "#\n" +
                "#  Generates a signed \"skinny\" JWT.\n" +
                "#  2 REST calls are made:\n" +
                "#  x1 pre-insert one classes\n" +
                "#  x1 pre-insert one object which uses previously inserted class\n" +
                "#\n" +
                "#  This JWT can be used in JS web button.\n" +
                "#  This is the shortest type of JWT; recommended for Android intents/redirects.\n" +
                "#\n" +
                "#############################\n"
            );

            Services services = new Services();
            String skinnyJwt = services.makeSkinnyJwt(verticalType, classId, objectId);

            if (skinnyJwt != null)
            {
                System.Console.WriteLine($"This is a \"skinny\" jwt:\n{skinnyJwt}\n");
                System.Console.WriteLine("you can decode it with a tool to see the unsigned JWT representation:\nhttps://jwt.io\n");
                System.Console.WriteLine($"Try this save link in your browser:\n{SAVE_LINK}{skinnyJwt}\n");
                System.Console.WriteLine("this is the shortest type of JWT; recommended for Android intents/redirects\n");
            }
            return;
        }

        [NoDirectAccess]
        public IActionResult Welcome() {
            return View();
        }
    }
 
}
