using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VABOrganizer
{
  public static class AvailablePartExtension
  {
    
    public static BulkheadData MaxSizeBulkheadData(this AvailablePart instance)
    {
      BulkheadData dataToReturn = new BulkheadData();
      if (!String.IsNullOrEmpty(instance.bulkheadProfiles))
      {
        string[] profileStrings = instance.bulkheadProfiles.Split(',').Select(s => s.Trim()).ToArray();
        float maxSize = 0f;

        for (int i = 0; i < profileStrings.Length; i++)
        {
          BulkheadData validData;
          if (BulkheadSorting.BulkheadMap.TryGetValue(profileStrings[i], out validData))
          {
            if (validData.Size > maxSize)
            {
              maxSize = validData.Size;
              dataToReturn = validData;
            }
          }
        }
      }
      return dataToReturn;
    
    }
  }
}
