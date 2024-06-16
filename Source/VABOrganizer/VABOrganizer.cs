using UnityEngine;
using UnityEngine.UI;
using KSP.UI;
using KSP.UI.TooltipTypes;
using KSP.UI.Screens;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;

namespace VABOrganizer
{
  [KSPAddon(KSPAddon.Startup.EditorAny, true)]
  public class VABOrganizer : MonoBehaviour
  {
    public static VABOrganizer Instance { get; private set; }
    public void Awake()
    {
      Settings.Load();
      BulkheadSorting.Load();
      SubcategorySorting.Load();
    }
  }

  [KSPAddon(KSPAddon.Startup.Instantly, true)]
  public class VABOrganizerAssets : MonoBehaviour
  {
    public static Dictionary<string,Sprite> Sprites { get; private set; }
    
    private void Awake()
    {

      Utils.Log("[UILoader]: Loading UI Prefabs");
      AssetBundle prefabs = AssetBundle.LoadFromFile(Path.Combine(KSPUtil.ApplicationRootPath, "GameData/VABOrganizer/Assets/vaborganizer.dat"));


      Sprite[] spriteSheet = prefabs.LoadAssetWithSubAssets<Sprite>("vab-organizer");
      Sprites = new Dictionary<string, Sprite>();
      foreach (Sprite subSprite in spriteSheet)
      {
        Sprites.Add(subSprite.name, subSprite);
      }
      
      Utils.Log("[UILoader]: Loaded UI Prefabs");
    }
  }
}
