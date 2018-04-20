using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;

namespace Microsoft.Bot.Sample.ProactiveBot
{
    [Serializable]
    public class TicketModel : LuisDialog<object>
    {
        //Prompts for the form flow question generation

        [Prompt(new string[] { "What is your name ? " })]
        public string Name { get; set; }

        /*[Prompt(new string[] { "Tell me, How can I assist you?" })]
        public string Assist { get; set; }*/

        [Prompt(new string[] { "Give a brief description of your problem" })]
        public string Desc { get; set; }

        /*[Prompt(new string[] { "To set the priority for your ticket, tell me about how many people are affected with the problem" })]
        public string Priority { get; set; }*/

        [Prompt(new string[] { "Enter your email address ? " })]
        public string Contact { get; set; }

        [Prompt(new string[] { "Enter your contact number"})]
        public string PhoneContact { get; set; }
          
        public string ServerName {get; set;}

        [Prompt(new string[] { "What middleware services are being used by you ? "})]
        public string MiddlewareName { get; set; }

        [Prompt(new string[] { "Which database are used by you ? "})]
        public string DatabaseName { get; set; }

        [Prompt(new string[] { "Please select a category (Inquiry/Help, Software, Hadware, Network, Database)"})]
        public string CategoryName { get; set; }

        public static IForm<TicketModel> BuildForm()
        {
            //Form flow builder being called

            return new FormBuilder<TicketModel>()
                .Field(nameof(Name), validate: ValidateNameInfo)
                .Field(nameof(Desc))
                .Field(nameof(ServerName)/*validate: ValidateServerInfo*/)
                .Field(nameof(MiddlewareName), validate: ValidateMiddlewareInfo)
                .Field(nameof(DatabaseName), validate: ValidateDatabaseInfo)
                .Field(nameof(CategoryName))
                //.Field(nameof(Priority))
                .Field(nameof(Contact), validate: ValidateContactInformation)
                .Field(nameof(PhoneContact), validate: ValidatePhoneContact)
                .AddRemainingFields()
                .Message("According to the responses entered by you I have generated a statement for you that showscase you problem : " +
                 "{Desc} running on server {ServerName}, using {DatabaseName} database and the {MiddlewareName} services used by you.")
                //"Please enter Yes if this successfully describe your problem.")
                .Build();
        }

        private static Task<ValidateResult> ValidatePhoneContact(TicketModel state, object value)
        {
            var result = new ValidateResult();
            string PhoneInfo = string.Empty;
            if (GetContactInfo((string)value, out PhoneInfo))
            {
                result.IsValid = true;
                result.Value = PhoneInfo;
                
            }

            else
            {
                result.IsValid = false;
                result.Feedback = "Please enter a valid contact number";
            }


            /*var heroCard = new HeroCard
            {
                Text = "Do you want us to contact you?",
                Buttons = new List<CardAction> { new CardAction(ActionTypes.MessageBack, "Call me!", value : "We will call you within 15 mins"),
                   new CardAction(ActionTypes.MessageBack, "Don't Call", value:"Ok")}
            };
            Attachment attachment = heroCard.ToAttachment();*/
            return Task.FromResult(result);
        }

        private static Task<ValidateResult> ValidateDatabaseInfo(TicketModel state, object value)
        {
            var result = new ValidateResult();
            string DatabaseInfo = string.Empty;
            if(GetDatabaseInfo((string)value, out DatabaseInfo))
            {
                result.IsValid = true;
                result.Value = DatabaseInfo;
            }
            else
            {
                result.IsValid = false;
                result.Feedback = "Please enter a valid database information";
            }
            return Task.FromResult(result);
        }

        private static Task<ValidateResult> ValidateMiddlewareInfo(TicketModel state, object value)
        {
            var result = new ValidateResult();
            string MiddlewareInfo = string.Empty;
            if (GetMiddlewareInfo((string)value, out MiddlewareInfo))
            {
                result.IsValid = true;
                result.Value = MiddlewareInfo;
            }
            else
            {
                result.IsValid = false;
                result.Feedback = "Please enter a valid Server information";
            }
            return Task.FromResult(result);
        }

        private static Task<ValidateResult> ValidateServerInfo(TicketModel state, object value)
        {
            var result = new ValidateResult();
            string ServerInfo = string.Empty;
            if(GetServerInfo((string)value, out ServerInfo))
            {
                result.IsValid = true;
                result.Value = ServerInfo;
            }
            else
            {
                result.IsValid = false;
                result.Feedback = "Please enter a valid Server name";
            }
            return Task.FromResult(result);
        }

        private static Task<ValidateResult> ValidateDescInfo(TicketModel state, object value)
        {
            var result = new ValidateResult();
            string descInfo = string.Empty;
            if(GetDescription((string)value, out descInfo))
            {
                result.IsValid = true;
                result.Value = descInfo;
            }
            else
            {
                result.IsValid = false;
                result.Feedback = "Please enter a valid response";
            }
            return Task.FromResult(result);
        }

        private static Task<ValidateResult> ValidateContactInformation(TicketModel state, object value)
        {
            var result = new ValidateResult();
            string contactInfo = string.Empty;
            if(GetEmailAddress((string)value, out contactInfo))
            {
                result.IsValid = true;
                result.Value = contactInfo;
            }
            else
            {
                result.IsValid = false;
                result.Feedback = "You did not enter a valid email address";
               
            }

            return Task.FromResult(result);
        }

        private static Task<ValidateResult> ValidateNameInfo(TicketModel state, object value)
        {
            var result = new ValidateResult();
            string usernameInfo = string.Empty;
            if(GetUsername((string)value, out usernameInfo))
            {
                result.IsValid = true;
                result.Value = usernameInfo;
            }
            else
            {
                result.IsValid = false;
                result.Feedback = "Please enter name a valid name";
            }
            return Task.FromResult(result);
        }

        private static bool DescriptionEnabled(TicketModel state) =>
             !string.IsNullOrWhiteSpace(state.ServerName) && !string.IsNullOrWhiteSpace(state.Name) &&  !string.IsNullOrWhiteSpace(state.Desc) && 
             !string.IsNullOrWhiteSpace(state.MiddlewareName) && !string.IsNullOrWhiteSpace(state.DatabaseName);

        private static bool GetEmailAddress(string response, out string contactInfo)
        {
            contactInfo = string.Empty;
            var match = Regex.Match(response, @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");
            if (match.Success)
            {
                contactInfo = match.Value;
                return true;
            }
            return false;
        }

        private static bool GetUsername(string value, out string usernameInfo)
        {
            usernameInfo = string.Empty;
            
            //Matches wether the if the name should start with a letter, should contain smaller case letter and should be 3 to 11 character long

            var match = Regex.Match(value, "[a-zA-z][a-z]{3,11}");
            if (match.Success)
            {
                usernameInfo = match.Value;
                return true;
            }
            return false;
        }

        private static bool GetDescription(string value, out string descInfo)
        {
            descInfo = string.Empty;

            var match = Regex.Match(value, "^[a-zA-z]*$");
            if(match.Success)
            {
                descInfo = match.Value;
                return true;
            }
            return false;
        }

        private static bool GetServerInfo(string value, out string serverInfo)
        {
            serverInfo = string.Empty;

            var match = Regex.Match(value, "^[a-zA-Z]");
            if (match.Success)
            {
                serverInfo = match.Value;
                return true;
            }
            return false;
        }

        private static bool GetMiddlewareInfo(string value, out string middlewareInfo)
        {
            middlewareInfo = string.Empty;

            var match = Regex.Match(value, "^[a-zA-z$_]+");
            if (match.Success)
            {
                middlewareInfo = match.Value;
                return true;
            }
            return false;
        }

        private static bool GetDatabaseInfo(string value, out string databaseInfo)
        {
            databaseInfo = string.Empty;

            var match = Regex.Match(value, "[0-9a-zA-Z$_]+");
            if (match.Success)
            {
                databaseInfo = match.Value;
                return true;
            }
            return false;
        }

        private static bool GetContactInfo(string value, out string phoneInfo)
        {
            phoneInfo = string.Empty;

            var match = Regex.Match(value, @"^[0-9]{10}$");
            if(match.Success)
            {
                phoneInfo = match.Value;
                return true;
            }
            return false;
        }
    }
}