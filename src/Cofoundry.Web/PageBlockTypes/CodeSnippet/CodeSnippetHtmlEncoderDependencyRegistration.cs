using Cofoundry.Core.AutoUpdate.Internal;
using Cofoundry.Core.AutoUpdate;
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web.PageBlockTypes.CodeSnippet;
public class CodeSnippetHtmlEncoderDependencyRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        container
            .RegisterAll<ICodeSnippetHtmlEncoder>(RegistrationOptions.SingletonScope())
            ;
    }
}
