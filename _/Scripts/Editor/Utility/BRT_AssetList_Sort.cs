using System;

namespace BuildReportRemastered
{
    public static partial class AssetListUtility
    {
        public static void SortAssetList(BuildReportRemastered.SizePart[] assetList, BuildReportRemastered.AssetList.SortType sortType, BuildReportRemastered.AssetList.SortOrder sortOrder)
        {
            switch (sortType)
            {
                case BuildReportRemastered.AssetList.SortType.RawSize:
                    SortRawSize(assetList, sortOrder);
                    break;
                case BuildReportRemastered.AssetList.SortType.ImportedSize:
                    SortImportedSize(assetList, sortOrder);
                    break;
                case BuildReportRemastered.AssetList.SortType.ImportedSizeOrRawSize:
                    SortImportedSizeOrRawSize(assetList, sortOrder);
                    break;
                case BuildReportRemastered.AssetList.SortType.SizeBeforeBuild:
                    SortSizeBeforeBuild(assetList, sortOrder);
                    break;
                case BuildReportRemastered.AssetList.SortType.PercentSize:
                    SortPercentSize(assetList, sortOrder);
                    break;
                case BuildReportRemastered.AssetList.SortType.AssetFullPath:
                    SortAssetFullPath(assetList, sortOrder);
                    break;
                case BuildReportRemastered.AssetList.SortType.AssetFilename:
                    SortAssetName(assetList, sortOrder);
                    break;
            }
        }

        static void SortRawSize(BuildReportRemastered.SizePart[] assetList, BuildReportRemastered.AssetList.SortOrder sortOrder)
        {
            if (sortOrder == BuildReportRemastered.AssetList.SortOrder.Descending)
            {
                Array.Sort(assetList, delegate (BuildReportRemastered.SizePart entry1, BuildReportRemastered.SizePart entry2)
                {
                    if (entry1.UsableSize > entry2.UsableSize) return -1;
                    if (entry1.UsableSize < entry2.UsableSize) return 1;

                    // same size
                    // sort by asset name for assets with same sizes
                    return SortByAssetNameDescending(entry1, entry2);
                });
            }
            else
            {
                Array.Sort(assetList, delegate (BuildReportRemastered.SizePart entry1, BuildReportRemastered.SizePart entry2)
                {
                    if (entry1.UsableSize > entry2.UsableSize) return 1;
                    if (entry1.UsableSize < entry2.UsableSize) return -1;

                    // same size
                    // sort by asset name for assets with same sizes
                    return SortByAssetNameAscending(entry1, entry2);
                });
            }
        }

        static void SortImportedSizeOrRawSize(BuildReportRemastered.SizePart[] assetList, BuildReportRemastered.AssetList.SortOrder sortOrder)
        {
            if (sortOrder == BuildReportRemastered.AssetList.SortOrder.Descending)
            {
                Array.Sort(assetList, delegate (BuildReportRemastered.SizePart entry1, BuildReportRemastered.SizePart entry2)
                {
                    if (entry1.ImportedSizeOrRawSize > entry2.ImportedSizeOrRawSize) return -1;
                    if (entry1.ImportedSizeOrRawSize < entry2.ImportedSizeOrRawSize) return 1;

                    // same size
                    // sort by asset name for assets with same sizes
                    return SortByAssetNameDescending(entry1, entry2);
                });
            }
            else
            {
                Array.Sort(assetList, delegate (BuildReportRemastered.SizePart entry1, BuildReportRemastered.SizePart entry2)
                {
                    if (entry1.ImportedSizeOrRawSize > entry2.ImportedSizeOrRawSize) return 1;
                    if (entry1.ImportedSizeOrRawSize < entry2.ImportedSizeOrRawSize) return -1;

                    // same size
                    // sort by asset name for assets with same sizes
                    return SortByAssetNameAscending(entry1, entry2);
                });
            }
        }

        static void SortImportedSize(BuildReportRemastered.SizePart[] assetList, BuildReportRemastered.AssetList.SortOrder sortOrder)
        {
            if (sortOrder == BuildReportRemastered.AssetList.SortOrder.Descending)
            {
                Array.Sort(assetList, delegate (BuildReportRemastered.SizePart entry1, BuildReportRemastered.SizePart entry2)
                {
                    if (entry1.ImportedSizeBytes > entry2.ImportedSizeBytes) return -1;
                    if (entry1.ImportedSizeBytes < entry2.ImportedSizeBytes) return 1;

                    // same size
                    // sort by asset name for assets with same sizes
                    return SortByAssetNameDescending(entry1, entry2);
                });
            }
            else
            {
                Array.Sort(assetList, delegate (BuildReportRemastered.SizePart entry1, BuildReportRemastered.SizePart entry2)
                {
                    if (entry1.ImportedSizeBytes > entry2.ImportedSizeBytes) return 1;
                    if (entry1.ImportedSizeBytes < entry2.ImportedSizeBytes) return -1;

                    // same size
                    // sort by asset name for assets with same sizes
                    return SortByAssetNameAscending(entry1, entry2);
                });
            }
        }

        static void SortSizeBeforeBuild(BuildReportRemastered.SizePart[] assetList, BuildReportRemastered.AssetList.SortOrder sortOrder)
        {
            if (sortOrder == BuildReportRemastered.AssetList.SortOrder.Descending)
            {
                Array.Sort(assetList, delegate (BuildReportRemastered.SizePart entry1, BuildReportRemastered.SizePart entry2)
                {
                    if (entry1.SizeInAssetsFolderBytes > entry2.SizeInAssetsFolderBytes) return -1;
                    if (entry1.SizeInAssetsFolderBytes < entry2.SizeInAssetsFolderBytes) return 1;

                    // same size
                    // sort by asset name for assets with same sizes
                    return SortByAssetNameDescending(entry1, entry2);
                });
            }
            else
            {
                Array.Sort(assetList, delegate (BuildReportRemastered.SizePart entry1, BuildReportRemastered.SizePart entry2)
                {
                    if (entry1.SizeInAssetsFolderBytes > entry2.SizeInAssetsFolderBytes) return 1;
                    if (entry1.SizeInAssetsFolderBytes < entry2.SizeInAssetsFolderBytes) return -1;

                    // same size
                    // sort by asset name for assets with same sizes
                    return SortByAssetNameAscending(entry1, entry2);
                });
            }
        }

        static void SortPercentSize(BuildReportRemastered.SizePart[] assetList, BuildReportRemastered.AssetList.SortOrder sortOrder)
        {
            if (sortOrder == BuildReportRemastered.AssetList.SortOrder.Descending)
            {
                Array.Sort(assetList, delegate (BuildReportRemastered.SizePart entry1, BuildReportRemastered.SizePart entry2)
                {
                    if (entry1.Percentage > entry2.Percentage) return -1;
                    if (entry1.Percentage < entry2.Percentage) return 1;

                    // same percent
                    // sort by asset name for assets with same percent
                    return SortByAssetFullPathDescending(entry1, entry2);
                });
            }
            else
            {
                Array.Sort(assetList, delegate (BuildReportRemastered.SizePart entry1, BuildReportRemastered.SizePart entry2)
                {
                    if (entry1.Percentage > entry2.Percentage) return 1;
                    if (entry1.Percentage < entry2.Percentage) return -1;

                    // same size
                    // sort by asset name for assets with same sizes
                    return SortByAssetFullPathAscending(entry1, entry2);
                });
            }
        }

        static void SortAssetFullPath(BuildReportRemastered.SizePart[] assetList, BuildReportRemastered.AssetList.SortOrder sortOrder)
        {
            if (sortOrder == BuildReportRemastered.AssetList.SortOrder.Descending)
            {
                Array.Sort(assetList, SortByAssetFullPathDescending);
            }
            else
            {
                Array.Sort(assetList, SortByAssetFullPathAscending);
            }
        }

        static int SortByAssetFullPathDescending(BuildReportRemastered.SizePart entry1, BuildReportRemastered.SizePart entry2)
        {
            int result = string.Compare(entry1.Name, entry2.Name, StringComparison.OrdinalIgnoreCase);

            return result;
        }

        static int SortByAssetFullPathAscending(BuildReportRemastered.SizePart entry1, BuildReportRemastered.SizePart entry2)
        {
            int result = string.Compare(entry1.Name, entry2.Name, StringComparison.OrdinalIgnoreCase);

            // invert the result
            if (result == 1) return -1;
            if (result == -1) return 1;
            return 0;
        }

        static void SortAssetName(BuildReportRemastered.SizePart[] assetList, BuildReportRemastered.AssetList.SortOrder sortOrder)
        {
            if (sortOrder == BuildReportRemastered.AssetList.SortOrder.Descending)
            {
                Array.Sort(assetList, SortByAssetNameDescending);
            }
            else
            {
                Array.Sort(assetList, SortByAssetNameAscending);
            }
        }

        static int SortByAssetNameDescending(BuildReportRemastered.SizePart entry1, BuildReportRemastered.SizePart entry2)
        {
            int result = string.Compare(entry1.Name.GetFileNameOnly(), entry2.Name.GetFileNameOnly(),
                StringComparison.OrdinalIgnoreCase);

            return result;
        }

        static int SortByAssetNameAscending(BuildReportRemastered.SizePart entry1, BuildReportRemastered.SizePart entry2)
        {
            int result = string.Compare(entry1.Name.GetFileNameOnly(), entry2.Name.GetFileNameOnly(),
                StringComparison.OrdinalIgnoreCase);

            // invert the result
            if (result == 1) return -1;
            if (result == -1) return 1;

            return 0;
        }
    }
}