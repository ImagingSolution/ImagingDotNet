using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagingSolution
{
    /// <summary>
    /// ImagingDotNetクラス
    /// </summary>
    public static partial class ImagingDotNet
    {
        // enum ThresholdTypes 相当の定数
        public const int THRESH_BINARY = 0; //!< \f[\texttt{dst} (x,y) =  \fork{\texttt{maxval}}{if \(\texttt{src}(x,y) > \texttt{thresh}\)}{0}{otherwise}\f]
        public const int THRESH_BINARY_INV = 1; //!< \f[\texttt{dst} (x,y) =  \fork{0}{if \(\texttt{src}(x,y) > \texttt{thresh}\)}{\texttt{maxval}}{otherwise}\f]
        public const int THRESH_TRUNC = 2; //!< \f[\texttt{dst} (x,y) =  \fork{\texttt{threshold}}{if \(\texttt{src}(x,y) > \texttt{thresh}\)}{\texttt{src}(x,y)}{otherwise}\f]
        public const int THRESH_TOZERO = 3; //!< \f[\texttt{dst} (x,y) =  \fork{\texttt{src}(x,y)}{if \(\texttt{src}(x,y) > \texttt{thresh}\)}{0}{otherwise}\f]
        public const int THRESH_TOZERO_INV = 4; //!< \f[\texttt{dst} (x,y) =  \fork{0}{if \(\texttt{src}(x,y) > \texttt{thresh}\)}{\texttt{src}(x,y)}{otherwise}\f]
        public const int THRESH_MASK = 7;
        public const int THRESH_OTSU = 8; //!< flag, use Otsu algorithm to choose the optimal threshold value
        public const int THRESH_TRIANGLE = 16; //!< flag, use Triangle algorithm to choose the optimal threshold value
    }
}
