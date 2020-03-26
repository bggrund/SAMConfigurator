using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;

namespace SAMConfig
{
   [AddINotifyPropertyChangedInterface]
   public class WindowViewModel
   {
      public PNDATR_ViewModel PNDDEF_ViewModel { get; set; }
      public PNDATR_ViewModel ATRDEF_ViewModel { get; set; }
      public PCKPRK_ViewModel PCKLST_ViewModel { get; set; }
      public PCKPRK_ViewModel PRKLST_ViewModel { get; set; }
      public BMPCLS_ViewModel BMPAGV_ViewModel { get; set; }
      public BMPCLS_ViewModel CLSAGV_ViewModel { get; set; }
      public OPCMgr_ViewModel OPCMgr_ViewModel { get; set; }

      public WindowViewModel()
      {
         PNDDEF_ViewModel = new PNDATR_ViewModel(FileType.PND);
         ATRDEF_ViewModel = new PNDATR_ViewModel(FileType.ATR);
         PCKLST_ViewModel = new PCKPRK_ViewModel(FileType.PCK);
         PRKLST_ViewModel = new PCKPRK_ViewModel(FileType.PRK);
         BMPAGV_ViewModel = new BMPCLS_ViewModel(FileType.BMP);
         CLSAGV_ViewModel = new BMPCLS_ViewModel(FileType.CLS);
         OPCMgr_ViewModel = new OPCMgr_ViewModel();
      }
   }
}
