using Autofac;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Scorables;
using Microsoft.Bot.Connector;
using ProactiveBot.Dialogs.ScorableDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace ProactiveBot.Modules
{
    public class IscorableRegisterModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            /**
             * registering builder for CancelScrolable
             **/

            builder
                .Register(c => new CancelScorable(c.Resolve<IDialogTask>()))
                .As<IScorable<IActivity, double>>()
                .InstancePerLifetimeScope();

            /**
             * Registering builder for VMConfigScroable
             **/

            builder
                .Register(c => new VirtualMachineScroable(c.Resolve<IDialogTask>()))
                .As<IScorable<IActivity, double>>()
                .InstancePerLifetimeScope();

            /**
             * Registering builder for RaiseTicketDialog
             **/

            /*builder
                .Register(c => new RaiseTicketScorable(c.Resolve<IDialogTask>()))
                .As<IScorable<IActivity, double>>()
                .InstancePerLifetimeScope();*/

            /**
             * Registering builder for RaiseIssueScorable
             **/

            builder
                .Register(c => new RaiseIssueScorable(c.Resolve<IDialogTask>()))
                .As<IScorable<IActivity, double>>()
                .InstancePerLifetimeScope();

            /**
             * Registering builder for StatusScorableDialog
             **/

            builder
                .Register(c => new StatusScorableDialog(c.Resolve<IDialogTask>()))
                .As<IScorable<IActivity, double>>()
                .InstancePerLifetimeScope();

            builder
                .Register(c => new ServerPasswordReset(c.Resolve<IDialogTask>()))
                .As<IScorable<IActivity, double>>()
                .InstancePerLifetimeScope();

        }
    }
}