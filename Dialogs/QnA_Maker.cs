using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web; 
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;

namespace ProactiveBot.Dialogs
{
    [Serializable]
    public class QnA_Maker : QnAMakerDialog
    {
        public QnA_Maker() : base(new QnAMakerService(new
            QnAMakerAttribute(ConfigurationManager.AppSettings["QnaSubscriptionkey"], 
            ConfigurationManager.AppSettings["QnaKnowledgebaseId"], 
            "Sorry, I could't find an answer for that",0.5)))
        {
        }
    }
}