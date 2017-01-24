using Discord.Commands;
using Discord.WebSocket;
using Selfbot.Modules.Addons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Selfbot.Attributes
{
    public class RequiredAttribute : ParameterPreconditionAttribute
    {
        public string Text { get; private set; }
        public RequiredAttribute(string text = null)
        {
            Text = text;
        }

        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, ParameterInfo parameter, object value, IDependencyMap map)
        {
            if (!(context is SelfbotCommandContext))
                return Task.FromResult(PreconditionResult.FromSuccess());
            if (value != null && !(value is object[]) || (value is object[] && (value as object[]).Count() != 0))
                return Task.FromResult(PreconditionResult.FromSuccess());

            SelfbotCommandContext con = context as SelfbotCommandContext;
            string cmdline;
            if (Text == null)
                cmdline = $"**Usage**: {con.MainHandler.Prefix}{parameter.Command.Aliases.First()} [{String.Join("] [", parameter.Command.Parameters.Select(x => x.Name))}]";
            else
                cmdline = $"**Usage**: {con.MainHandler.Prefix}{parameter.Command.Aliases.First()} [{Text}]";
            return Task.FromResult(PreconditionResult.FromError(cmdline));
        }
    }
}
