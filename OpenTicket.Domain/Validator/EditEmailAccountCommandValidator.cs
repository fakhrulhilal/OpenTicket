﻿using FluentValidation;
using OpenTicket.Domain.Command;
using OpenTicket.Domain.MailClient;

namespace OpenTicket.Domain.Validator
{
    public class EditEmailAccountCommandValidator : AbstractValidator<EditEmailAccountCommand>
    {
        public EditEmailAccountCommandValidator()
        {
            RuleFor(p => p.Id).GreaterThan(0);
            RuleFor(p => p.Email).NotEmpty().MaximumLength(100);
            RuleFor(p => p.UserId).NotEmpty().MaximumLength(250);
            RuleFor(p => p.ServerAddress).NotEmpty().When(cmd => cmd.Protocol != MailProtocolType.M365);
            RuleFor(p => p.ServerPort).GreaterThan(0).When(cmd => cmd.Protocol != MailProtocolType.M365);
        }
    }
}
