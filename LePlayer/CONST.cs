using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LePlayer
{
    public class CONST
    {
        public static ContextOption DIC_WORD_CRAWLE =
            new ContextOption(new FORM_TYPE[] {
                FORM_TYPE.DICTIONARY
            }, new CONTEXT_ACTION[] {
                CONTEXT_ACTION.CRAWLE_CAMBRIDGE___DIC_WORD
            });


    }
}
