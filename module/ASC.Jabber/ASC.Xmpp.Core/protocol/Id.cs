/*
 *
 * (c) Copyright Ascensio System Limited 2010-2015
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


#region file header

#endregion

using System;

namespace ASC.Xmpp.Core.protocol
{

    #region usings

    #endregion

    /// <summary>
    /// </summary>
    public enum IdType
    {
        /// <summary>
        ///   Numeric Id's are generated by increasing a long value
        /// </summary>
        Numeric,

        /// <summary>
        ///   Guid Id's are unique, Guid packet Id's should be used for server and component applications, or apps which very long sessions (multiple days, weeks or years)
        /// </summary>
        Guid
    }

    /// <summary>
    ///   This class takes care anout out unique Message Ids
    /// </summary>
    public class Id
    {
        /// <summary>
        /// </summary>
        private static long m_id;

        /// <summary>
        /// </summary>
        private static string m_Prefix = "agsXMPP_";

        /// <summary>
        /// </summary>
        private static IdType m_Type = IdType.Numeric;

        /// <summary>
        /// </summary>
        public static IdType Type
        {
            get { return m_Type; }

#if !CF
            // readyonly on CF1
            set { m_Type = value; }

#endif
        }

#if !CF

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public static string GetNextId()
        {
            if (m_Type == IdType.Numeric)
            {
                m_id++;
                return m_Prefix + m_id;
            }
            else
            {
                return m_Prefix + Guid.NewGuid();
            }
        }

#else
        
    
    
    // On CF 1.0 we have no GUID class, so only increasing numberical id's are supported
    // We could create GUID's on CF 1.0 with the Crypto API if we want to.
        public static string GetNextId()
        {            
            m_id++;
            return m_Prefix + m_id.ToString();
        }

#endif

        /// <summary>
        ///   Reset the id counter to agsXmpp_1 again
        /// </summary>
        public static void Reset()
        {
            m_id = 0;
        }

        /// <summary>
        ///   to Save Bandwidth on Mobile devices you can change the prefix null is also possible to optimize Bandwidth usage
        /// </summary>
        public static string Prefix
        {
            get { return m_Prefix; }

            set
            {
                if (value == null)
                {
                    m_Prefix = string.Empty;
                }
                else
                {
                    m_Prefix = value;
                }
            }
        }
    }
}