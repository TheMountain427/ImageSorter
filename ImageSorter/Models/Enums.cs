

namespace ImageSorter.Models;

public static class Enums
{
    public enum ImgOrder
    {
        AscFileName = 1,
        DescFileName = 2,
        AscFileSize = 3,
        DescFileSize = 4,
        AscFileCreatedTime = 5,
        DescFileCreatedTime = 6,
        AscLastModifiedTime = 7,
        DescLastModifiedTime = 8,
        AscFilterValue= 9,
        DescFilterValue = 10
    }

    public enum ReferenceViewIdentifier
    {
        Alpha = 0,
        Beta = 1
    }

    public enum ReferenceSplit
    {
        Split = 0,
        Alpha = 1,
        Beta = 2
    }
}