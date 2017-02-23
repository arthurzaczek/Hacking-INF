using System;
using System.Collections.Generic;
using Novell.Directory.Ldap;
using System.Linq;
using System.Net;
using System.Web;

namespace Hacking_INF.Providers
{
    public class LDAPUser
    {
        public LDAPUser()
        {
            this.IsAuthenticated = false;
        }

        public LDAPUser(bool auth, string firstname, string lastname, string fullname, string email, string studiengang, string studiengangKuerzel, string personal)
        {
            this.IsAuthenticated = auth;
            this.Firstname = firstname;
            this.Lastname = lastname;
            this.Fullname = fullname;
            this.Mail = email;
            this.Studiengang = studiengang;
            this.StudiengangKuerzel = studiengangKuerzel;
            this.PersonalType = personal;
        }

        public bool IsAuthenticated { get; private set; }

        public string Mail { get; private set; }
        public string Firstname { get; private set; }
        public string Lastname { get; private set; }
        public string Fullname { get; private set; }
        public string Studiengang { get; private set; }
        public string StudiengangKuerzel { get; private set; }
        public string PersonalType { get; private set; }
    }

    public class LDAPAuthenticator
    {
        const string ATTRIBUTES = "ou=People,dc=technikum-wien,dc=at";
        const string HOST = "ldap.technikum-wien.at";
        const int PORT = 389;
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(LDAPAuthenticator));

        private static bool? _disablePasswordCheck;
        public static bool DisablePasswordCheck
        {
            get
            {
                if (_disablePasswordCheck == null)
                {
                    // set DEV_ENVIRONMENT = 1, done by IISExpress
                    _disablePasswordCheck = System.Environment.GetEnvironmentVariable("DEV_ENVIRONMENT") == "1";
                    _log.InfoFormat("DisablePasswordCheck = {0}", _disablePasswordCheck);
                }
                return _disablePasswordCheck.Value;
            }
        }

        private static bool? _startTLS;
        public static bool StartTLS
        {
            get
            {
                if (_startTLS == null)
                {
                    _startTLS = Properties.Settings.Default.ldap_start_tls;
                    _log.InfoFormat("StartTLS = {0}", _startTLS);
                }
                return _startTLS.Value;
            }
        }

        private static LDAPUser GetUserParameter(LdapConnection ldap, string username)
        {
            try
            {
                //user daten parameter lesen - frage: daten nur holen, wenn noch nicht registriert oder sollen die daten immer verwendet werden und u.U auch in sofi aktualisiert werden
                string[] attributesToReturn = new string[] { "displayName", "sn", "givenName", "cn", "mail", "ou", "gidNumber", "uid" };

                var result = new LDAPUser();
                string sMail = "";
                string sDisplayName = "";
                string sFirstName = "";
                string sLastName = "";
                string sStudiengang = "";
                string sStudiengangKuerzel = "";
                string sPersonalBezeichnung = "";

                var queue = ldap.Search(ATTRIBUTES, LdapConnection.SCOPE_SUB, string.Format("(uid={0})", username), attributesToReturn, false);

                LdapEntry entry = queue.next();
                if (entry != null)
                {
                    sMail = entry.getAttribute("mail")?.StringValue ?? string.Empty;
                    sLastName = entry.getAttribute("sn")?.StringValue ?? string.Empty;
                    sFirstName = entry.getAttribute("givenName")?.StringValue ?? string.Empty;
                    sDisplayName = entry.getAttribute("displayName")?.StringValue ?? string.Format("{0} {1}", sFirstName, sLastName);
                    string iGidNumber = entry.getAttribute("gidNumber")?.StringValue ?? string.Empty; // 101=Technikum, 102=Student

                    sStudiengang = "";
                    sStudiengangKuerzel = "";
                    sPersonalBezeichnung = "";
                    var oOu = entry.getAttribute("ou");
                    if (oOu != null && oOu.size() > 0)
                    {
                        switch (iGidNumber)
                        {
                            case "101": //tw-personal - in ou steht meist Teacher
                                sPersonalBezeichnung = oOu.StringValueArray[1];
                                break;
                            case "102": //student
                                sStudiengang = oOu.StringValueArray[1];
                                sStudiengangKuerzel = oOu.StringValueArray[2];
                                break;
                        }
                    }
                }

                return new LDAPUser(true, sFirstName, sLastName, sDisplayName, sMail, sStudiengang, sStudiengangKuerzel, sPersonalBezeichnung);
            }
            catch (Exception ex)
            {
                _log.Warn("Unable to get ldap user attributes.", ex);
                return new LDAPUser();
            }
        }

        public static LDAPUser GetUserParameter(string username)
        {
            var ldap = Connect();
            try
            {
                ldap.Bind(null, null);
                return GetUserParameter(ldap, username);
            }
            finally
            {
                ldap.Disconnect();
            }
        }

        public static LDAPUser Authenticate(string username, string password)
        {
            var ldap = Connect();
            try
            {
                if (DisablePasswordCheck == false)
                {
                    ldap.Bind("uid=" + username + "," + ATTRIBUTES, password);
                }
                else
                {
                    ldap.Bind(null, null);
                }

                return GetUserParameter(ldap, username);
            }
            catch (Exception ex)
            {
                _log.Warn(string.Format("Unable to authenticate user {0}.", username), ex);
                return new LDAPUser();
            }
            finally
            {
                ldap.Disconnect();
            }
        }

        private static LdapConnection Connect()
        {
            var ldap = new LdapConnection();
            ldap.Connect(HOST, PORT);
            if (StartTLS)
            {
                ldap.startTLS();
            }
            return ldap;
        }
    }
}