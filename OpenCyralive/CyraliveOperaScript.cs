﻿using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCyralive
{
    internal class CyraliveOperaScript
    {
        public static List<string> CyraliveOperaScriptVar = new List<string>();
        public static List<string> CyraliveOperaScriptVarVal = new List<string>();

        public static void CyraliveOperaScript_init()
        {
            CyraliveOperaScriptVar.Add("$USERNAME$");
            CyraliveOperaScriptVarVal.Add(UserPrincipal.Current.DisplayName);
        }
    }
}
