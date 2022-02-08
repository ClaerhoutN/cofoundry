﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Full representation of a user, containing all properties. Users 
    /// are partitioned by user area so a user might be a Cofoundry admin 
    /// user or could belong to a custom user area. Users cannot belong to 
    /// more than one user area.
    /// </summary>
    public class UserDetails : UserMicroSummary, ICreateAudited
    {
        /// <summary>
        /// Each user must be assigned to a role which provides
        /// information about the actions a user is permitted to 
        /// perform.
        /// </summary>
        public RoleDetails Role { get; set; }

        /// <summary>
        /// The date the user last signed into the application. May be
        /// null if the user has not signed in yet.
        /// </summary>
        public DateTime? LastSignInDate { get; set; }

        /// <summary>
        /// The date the password was last changed or the that the password
        /// was first set (account create date)
        /// </summary>
        public DateTime LastPasswordChangeDate { get; set; }

        /// <summary>
        /// True if a password change is required, this is set to true when an account is
        /// first created.
        /// </summary>
        public bool RequirePasswordChange { get; set; }

        /// <summary>
        /// A generic verification date that can be used to mark an account as verified
        /// or activated. One common way of verification is via an email sign-up notification.
        /// </summary>
        public DateTime? AccountVerifiedDate { get; set; }

        /// <summary>
        /// Data detailing who created the user and when.
        /// </summary>
        public CreateAuditData AuditData { get; set; }
    }
}
