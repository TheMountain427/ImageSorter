using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static ImageSorter.Models.Helpers;
using static ImageSorter.Models.SortOptionKey;

namespace ImageSorter.Models
{
    public class SortEngine
    {
        public IEnumerable<IEnumerable<ImageDetails>> SortImageDetailsForOutput(IEnumerable<ImageDetails> InputImageDetails, IEnumerable<ImageDetails> ReferenceImageDetails, SortConfigs SortConfigs, ICommand ProgressIncrement)
        {
            ProgressIncrement.Execute(null);
            var referenceFiltersToSortBy = GetSortedImageFilters(ReferenceImageDetails).Distinct();

            ProgressIncrement.Execute(null);
            var inputImageFilters = GetSortedImageFilters(InputImageDetails).Distinct();

            ProgressIncrement.Execute(null);
            referenceFiltersToSortBy = referenceFiltersToSortBy.Union(inputImageFilters);


            // Filter down the image details based on settings
            ProgressIncrement.Execute(null);
            IQueryable<ImageDetails> sortQuery = InputImageDetails.AsQueryable(); 
            
            // Ignoring orphans means we only want things in the reference filter list
            ProgressIncrement.Execute(null);
            if (SortConfigs.GetKey<bool>(IgnoreOrphans).Value == true)
            {
                sortQuery = sortQuery.Where(x => x.FilteredValue.Any(y => y.Equals(referenceFiltersToSortBy)));
            }
            else if (SortConfigs.GetKey<bool>(IncludeOrphans).Value == true)
            {
                referenceFiltersToSortBy = referenceFiltersToSortBy.Union(inputImageFilters);
            }

            // Don't actually need to check IngnoreUnsorted
            ProgressIncrement.Execute(null);
            if (SortConfigs.GetKey<bool>(IgnoreUnsorted).Value == true)
            {
                sortQuery = sortQuery.Where(x => x.FilteredValue != "Unsorted");
                referenceFiltersToSortBy = referenceFiltersToSortBy.Where(x => x != "Unsorted");
            }
            
            // Fire the query and group the trimmed images together
            ProgressIncrement.Execute(null);
            var trimmedImagesGrouped = sortQuery.GroupBy(x => x.FilteredValue).Select(x => x.AsEnumerable());

            // ????? it uh, sorts them... and uh, puts them into groups...
            // returns IEnumerable<IIEnumerable<ImageDetails> where each inner IEnumerable contains all ImageDetails that have the same filter
            ProgressIncrement.Execute(null);
            var sortedImageDetails = referenceFiltersToSortBy.Where(filter => trimmedImagesGrouped.Any(imageGroup => imageGroup.Any(x => x.FilteredValue == filter)))
                                                       .Select(filter => trimmedImagesGrouped.First(imageGroup => imageGroup.First().FilteredValue == filter));



            ProgressIncrement.Execute(null);
            ProgressIncrement.Execute(null);
            return sortedImageDetails;

        }


        public SortEngine()
        {

        }
    }
}
