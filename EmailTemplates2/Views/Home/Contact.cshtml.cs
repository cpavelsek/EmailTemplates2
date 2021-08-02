using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using EmailTemplates2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmailTemplates2.Views.Home
{
    public class ContactModel : PageModel
    {
        private readonly ContactViewModel _context;

		public ContactModel( ContactViewModel context ) => _context = context;

		[BindProperty]
        public ContactViewModel contactViewModel { get; set; }

        public void OnGet()
        {
        }

        public async Task OnPostSendEmail() {
            using ( var smtp = new SmtpClient( "Your SMTP server address" ) ) {
                var emailMessage = new MailMessage();
                emailMessage.From = new MailAddress( contactViewModel.Email );
                emailMessage.To.Add( contactViewModel.Email );
                emailMessage.Subject = contactViewModel.Subject;
                emailMessage.Body = contactViewModel.Message;

                await smtp.SendMailAsync( emailMessage );
            }
        }

    }
}
