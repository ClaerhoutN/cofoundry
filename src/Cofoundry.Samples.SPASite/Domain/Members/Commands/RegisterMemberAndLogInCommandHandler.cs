﻿using Cofoundry.Core.Mail;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Samples.SPASite.Domain
{
    /// <summary>
    /// This command handler ties together a number of functions built
    /// into Cofoundry such as saving a new user, sending a notification
    /// and logging them in.
    /// </summary>
    public class RegisterMemberAndLogInCommandHandler
        : ICommandHandler<RegisterMemberAndLogInCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly IAdvancedContentRepository _contentRepository;

        private readonly ILoginService _loginService;
        private readonly IMailService _mailService;
        
        public RegisterMemberAndLogInCommandHandler(
            IAdvancedContentRepository contentRepository,
            ILoginService loginService,
            IMailService mailService
            )
        {
            _contentRepository = contentRepository;
            _loginService = loginService;
            _mailService = mailService;
        }

        public async Task ExecuteAsync(RegisterMemberAndLogInCommand command, IExecutionContext executionContext)
        {
            var addUserCommand = MapAddUserCommand(command);
            
            // Because we're not logged in, we'll need to elevate permissions to add a new user account.
            var userId = await _contentRepository
                .WithElevatedPermissions()
                .Users()
                .AddAsync(addUserCommand);

            await SendWelcomeNotification(command);

            await _loginService.LogAuthenticatedUserInAsync(addUserCommand.UserAreaCode, userId, true);
        }

        /// <summary>
        /// We're going to make use of the built in AddUserCommand which will take 
        /// care of most of the logic for us. Here we map from our domain command to 
        /// the Cofoundry one. 
        /// 
        /// It's important that we don't expose the AddUserCommand directly in our
        /// web api, which could allow a 'parameter injection attack' to take place:
        /// 
        /// See https://www.owasp.org/index.php/Web_Parameter_Tampering
        /// </summary>
        private AddUserCommand MapAddUserCommand(RegisterMemberAndLogInCommand command)
        {
            var addUserCommand = new AddUserCommand();
            addUserCommand.Email = command.Email;
            addUserCommand.FirstName = command.FirstName;
            addUserCommand.LastName = command.LastName;
            addUserCommand.Password = command.Password;
            addUserCommand.RoleCode = MemberRole.MemberRoleCode;
            addUserCommand.UserAreaCode = MemberUserArea.Code;

            return addUserCommand;
        }

        /// <summary>
        /// For more info on sending mail with Cofoundry see https://www.cofoundry.org/docs/framework/mail
        /// </summary>
        private async Task SendWelcomeNotification(RegisterMemberAndLogInCommand command)
        {
            var welcomeEmailTemplate = new NewUserWelcomeMailTemplate();
            welcomeEmailTemplate.FirstName = command.FirstName;
            await _mailService.SendAsync(command.Email, welcomeEmailTemplate);
        }
    }
}
