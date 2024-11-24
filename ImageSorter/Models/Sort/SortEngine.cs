using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using static ImageSorter.Models.Helpers;
using static ImageSorter.Models.SortOptionKey;

namespace ImageSorter.Models
{
    public class SortEngine
    {
        private ICommand? IncrementProgress { get; }

        //For if we want to sort without checking the progress 
        private void _incrementProgress()
        {
            if (this.IncrementProgress is not null)
            {
                IncrementProgress.Execute(null);
            }
        }

        public IEnumerable<IEnumerable<ImageDetails>> SortImageDetailsForOutput(IEnumerable<ImageDetails> InputImageDetails, IEnumerable<ImageDetails> ReferenceImageDetails, SortConfigs SortConfigs)
        {
            var referenceFiltersToSortBy = GetSortedImageFilters(ReferenceImageDetails).Distinct();

            _incrementProgress();

            var inputImageFilters = GetSortedImageFilters(InputImageDetails).Distinct();

            _incrementProgress();

            _incrementProgress();


            // Filter down the image details based on settings
            IQueryable<ImageDetails> sortQuery = InputImageDetails.AsQueryable();

            _incrementProgress();

            // Ignoring orphans means we only want things in the reference filter list
            if (SortConfigs.GetKey<bool>(IgnoreOrphans).Value == true)
            {
                //sortQuery = sortQuery.Where(x => x.FilteredValue.Any(y => y.Equals(referenceFiltersToSortBy)));
                sortQuery = sortQuery.Where(image => referenceFiltersToSortBy.Any(filter => image.FilteredValue == filter));
            }
            else if (SortConfigs.GetKey<bool>(IncludeOrphans).Value == true)
            {
                referenceFiltersToSortBy = referenceFiltersToSortBy.Union(inputImageFilters);
            }

            _incrementProgress();

            // Don't actually need to check IngnoreUnsorted
            if (SortConfigs.GetKey<bool>(IgnoreUnsorted).Value == true)
            {
                sortQuery = sortQuery.Where(x => x.FilteredValue != "Unsorted");
                referenceFiltersToSortBy = referenceFiltersToSortBy.Where(x => x != "Unsorted");
            }

            _incrementProgress();

            // Fire the query and group the trimmed images together
            var trimmedImagesGrouped = sortQuery.GroupBy(x => x.FilteredValue).Select(x => x.AsEnumerable());

            _incrementProgress();

            // ************************************************************************************
            // I think these are actually already grouped by this point and the next step is redundant
            // ************************************************************************************
            // ????? it uh, sorts them... and uh, puts them into groups...
            // returns IEnumerable<IIEnumerable<ImageDetails> where each inner IEnumerable contains all ImageDetails that have the same filter
            var sortedImageDetails = referenceFiltersToSortBy.Where(filter => trimmedImagesGrouped.Any(imageGroup => imageGroup.Any(x => x.FilteredValue == filter)))
                                                       .Select(filter => trimmedImagesGrouped.First(imageGroup => imageGroup.First().FilteredValue == filter));

            _incrementProgress();



            _incrementProgress();
            _incrementProgress();

            // Fix for progress increment, since max was calculated by InputImageDetails.Count()
            // but sortedImageDetails may be less than InputImageDetails.Count().
            // Without this, SortImagesIntoDirectories will fail to increment enough
            int progressFix = InputImageDetails.Count() - sortQuery.Count();
            for (int i = 0; i < progressFix; i++)
            {
                _incrementProgress();
            }

            return sortedImageDetails;

        }

        public IEnumerable<FilterDetails> CreateOutputDirectories(IEnumerable<FilterDetails> DirectoryNames, string OutputPath)
        {
            var outputDirectories = new List<FilterDetails>();

            // :)
            int i = 0;

            if (!string.IsNullOrEmpty(OutputPath) && Directory.Exists(OutputPath))
            {
                if (DirectoryNames.Any())
                {
                    foreach (var item in DirectoryNames)
                    {
                        if (!string.IsNullOrEmpty(item.DirectoryName))
                        {
                            var fullPath = Path.Join(OutputPath, item.DirectoryName);
                            outputDirectories.Add(new FilterDetails()
                            {
                                DirectoryName = item.DirectoryName,
                                FilterValue = item.FilterValue,
                                FullPath = fullPath
                            });

                            if (!Directory.Exists(fullPath))
                            {
                                Directory.CreateDirectory(fullPath);
                            }

                            // Since I decided this block is worth 8 progress points. Don't increment over 7.
                            if (i >= 7)
                            {
                                i++;
                                _incrementProgress();

                            }
                        }
                        else
                        {
                            throw new ArgumentException($"No value was given for an output directory");
                        }
                    }
                }
                else
                {
                    throw new ArgumentException($"No values were given for output directories");
                }
            }
            else
            {
                throw new ArgumentException($"Output path does not exist at {OutputPath}");
            }

            // This gets up to 8 progress points
            while (i < 8)
            {
                i++;
                _incrementProgress();
            }

            return outputDirectories;
        }

        public void SortImagesIntoDirectories(IEnumerable<IEnumerable<ImageDetails>> ImageDetailGroups, IEnumerable<FilterDetails> OutputDirectories)
        {
            // No safety checks lets send it
            foreach (var imageGroup in ImageDetailGroups)
            {
                var detail = imageGroup.First();
                var filterOutputPath = OutputDirectories.Where(filterDetail => filterDetail.FilterValue == detail.FilteredValue).First().FullPath;

                foreach (var image in imageGroup)
                {
                    var destination = Path.Join(filterOutputPath, image.FileName);
                    // Okay, 1 safety check
                    if (!File.Exists(destination))
                    {
                        File.Copy(image.FilePath, destination);
                    }

                    _incrementProgress();
                }
            }
        }


        // Hopefully a temp method to get FilterOutputDirectory's since I have not moved to using that fully
        public IEnumerable<FilterDetails> ConvertSortedGroupsToFilterOutputDirectories(IEnumerable<IEnumerable<ImageDetails>> ImageGroups)
        {
            var distinctFilters = ImageGroups.Where(group => group.Any())
                                             .Select(group => group.First().FilteredValue);

            var convertedFilters = new List<FilterDetails>();

            foreach (var filter in distinctFilters)
            {
                convertedFilters.Add(new FilterDetails(filter, filter));
            }

            return convertedFilters;
        }

        public double CalculateSortProgressMaximum(ProjectConfig ProjectConfig)
        {
            int baseMaximum = 0;

            int sortImageDetailsForOutput_Value = 10;

            int createOutputDirectories_Value = 8;

            int imageCount = ProjectConfig.InputImages.Count;




            double calculatedMax = new double[]
            {
                baseMaximum,
                sortImageDetailsForOutput_Value,
                createOutputDirectories_Value,
                imageCount
            }.Sum();

            return calculatedMax;
        }

        public SortEngine()
        {
        }

        public SortEngine(ICommand IncrementProgress)
        {
            this.IncrementProgress = IncrementProgress;
        }
    }
}
