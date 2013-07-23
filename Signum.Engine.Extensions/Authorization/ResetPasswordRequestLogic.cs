using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Entities.Authorization;
using System.Reflection;
using Signum.Engine.DynamicQuery;
using Signum.Engine.Maps;
using Signum.Entities;
using Signum.Engine.Mailing;
using Signum.Utilities;
using Signum.Entities.Mailing;
using Signum.Engine.Basics;
using System.Globalization;
using Signum.Engine.Translation;

namespace Signum.Engine.Authorization
{
    public static class ResetPasswordRequestLogic
    {
        public static void Start(SchemaBuilder sb, DynamicQueryManager dqm)
        {
            if (sb.NotDefined(MethodInfo.GetCurrentMethod()))
            {
                sb.Include<ResetPasswordRequestDN>();

                dqm.RegisterQuery(typeof(ResetPasswordRequestDN), () =>
                    from e in Database.Query<ResetPasswordRequestDN>()
                    select new
                    {
                        Entity = e,
                        e.Id,
                        e.RequestDate,
                        e.Code,
                        e.User,
                        e.User.Email
                    });

                EmailLogic.AssertStarted(sb);

                SystemEmailLogic.RegisterSystemEmail<ResetPasswordRequestMail>(() => new EmailTemplateDN
                {
                    Name = "Reset Password Request",
                    IsBodyHtml = true,
                    Messages = CultureInfoLogic.ForEachCulture(culture => new EmailTemplateMessageDN(culture)
                    {
                        Text = "<p>{0}</p>".Formato(AuthEmailMessage.YouRecentlyRequestedANewPassword.NiceToString()) + 
                            "<p>{0} @[User.UserName]</p>".Formato(AuthEmailMessage.YourUsernameIs.NiceToString()) +
                            "<p>{0}</p>".Formato(AuthEmailMessage.YouCanResetYourPasswordByFollowingTheLinkBelow.NiceToString()) +
                            "<p><a href=\"@model[Url]\">@model[Url]</a></p>",
                        Subject = AuthEmailMessage.ResetPasswordRequestSubject.NiceToString()
                    }).ToMList()
                });
            }
        }

        public class ResetPasswordRequestMail : SystemEmail<ResetPasswordRequestDN>
        {
            public string Url;

            public override List<EmailOwnerRecipientData> GetRecipients()
            {
                return SendTo(Entity.User.EmailOwnerData); 
            }
        }

        public static ResetPasswordRequestDN ResetPasswordRequest(UserDN user)
        {
            //Remove old previous requests
            Database.Query<ResetPasswordRequestDN>()
                .Where(r => r.User.Is(user) && r.RequestDate < TimeZoneManager.Now.AddMonths(1))
                .UnsafeDelete();

            return new ResetPasswordRequestDN()
            {
                Code = MyRandom.Current.NextString(5),
                User = user,
                RequestDate = TimeZoneManager.Now,
            }.Save();
        }

        public static void ResetPasswordRequestAndSendEmail(UserDN user, Func<ResetPasswordRequestDN, string> urlResetPassword)
        {
            var rpr = ResetPasswordRequest(user);
            var url = urlResetPassword(rpr);
            new ResetPasswordRequestMail { Entity = rpr, Url = url }.SendMailAsync();
        }

        public static Func<string, UserDN> GetUserByEmail = (email) =>
        {
            UserDN user = Database.Query<UserDN>().Where(u => u.Email == email).SingleOrDefaultEx();

            if (user == null)
                throw new ApplicationException(AuthMessage.ThereSNotARegisteredUserWithThatEmailAddress.NiceToString());

            return user;
        };

    }
}
