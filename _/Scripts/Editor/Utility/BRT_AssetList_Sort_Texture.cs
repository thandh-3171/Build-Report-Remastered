using System;

namespace BuildReportRemastered
{
    public static partial class AssetListUtility
    {
        public static void SortAssetList(BuildReportRemastered.SizePart[] assetList, BuildReportRemastered.TextureData textureData, TextureData.DataId textureSortType, BuildReportRemastered.AssetList.SortOrder sortOrder)
        {
            switch (textureSortType)
            {
                case BuildReportRemastered.TextureData.DataId.TextureType:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.TextureType);
                    break;
                case BuildReportRemastered.TextureData.DataId.IsSRGB:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.IsSRGB);
                    break;
                case BuildReportRemastered.TextureData.DataId.AlphaSource:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.AlphaSource);
                    break;
                case BuildReportRemastered.TextureData.DataId.AlphaIsTransparency:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.AlphaIsTransparency);
                    break;
                case BuildReportRemastered.TextureData.DataId.IgnorePngGamma:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.IgnorePngGamma);
                    break;
                case BuildReportRemastered.TextureData.DataId.NPotScale:
                    SortNPotScale(assetList, textureData, sortOrder);
                    break;
                case BuildReportRemastered.TextureData.DataId.IsReadable:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.IsReadable);
                    break;
                case BuildReportRemastered.TextureData.DataId.MipMapGenerated:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.MipMapGenerated);
                    break;
                case BuildReportRemastered.TextureData.DataId.MipMapFilter:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.MipMapFilter);
                    break;
                case BuildReportRemastered.TextureData.DataId.StreamingMipMaps:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.StreamingMipMaps);
                    break;
                case BuildReportRemastered.TextureData.DataId.BorderMipMaps:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.BorderMipMaps);
                    break;
                case BuildReportRemastered.TextureData.DataId.PreserveCoverageMipMaps:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.PreserveCoverageMipMaps);
                    break;
                case BuildReportRemastered.TextureData.DataId.FadeOutMipMaps:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.FadeOutMipMaps);
                    break;
                case BuildReportRemastered.TextureData.DataId.SpriteImportMode:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.SpriteImportMode);
                    break;
                case BuildReportRemastered.TextureData.DataId.SpritePackingTag:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.SpritePackingTag);
                    break;
                case BuildReportRemastered.TextureData.DataId.SpritePixelsPerUnit:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.SpritePixelsPerUnit);
                    break;
                case BuildReportRemastered.TextureData.DataId.QualifiesForSpritePacking:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.QualifiesForSpritePacking);
                    break;
                case BuildReportRemastered.TextureData.DataId.WrapMode:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.WrapMode);
                    break;
                case BuildReportRemastered.TextureData.DataId.WrapModeU:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.WrapModeU);
                    break;
                case BuildReportRemastered.TextureData.DataId.WrapModeV:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.WrapModeV);
                    break;
                case BuildReportRemastered.TextureData.DataId.WrapModeW:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.WrapModeW);
                    break;
                case BuildReportRemastered.TextureData.DataId.FilterMode:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.FilterMode);
                    break;
                case BuildReportRemastered.TextureData.DataId.AnisoLevel:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.AnisoLevel);
                    break;
                case BuildReportRemastered.TextureData.DataId.MaxTextureSize:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.GetShownMaxTextureSize());
                    break;
                case BuildReportRemastered.TextureData.DataId.TextureResizeAlgorithm:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.GetShownTextureResizeAlgorithm());
                    break;
                case BuildReportRemastered.TextureData.DataId.TextureFormat:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.GetShownTextureFormat());
                    break;
                case BuildReportRemastered.TextureData.DataId.CompressionType:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.GetShownCompressionType());
                    break;
                case BuildReportRemastered.TextureData.DataId.CompressionIsCrunched:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.GetShownCompressionIsCrunched());
                    break;
                case BuildReportRemastered.TextureData.DataId.CompressionQuality:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.GetShownCompressionQuality());
                    break;
                case BuildReportRemastered.TextureData.DataId.ImportedWidthAndHeight:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.GetImportedPixelCount());
                    break;
                case BuildReportRemastered.TextureData.DataId.RealWidthAndHeight:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.GetRealPixelCount());
                    break;
            }
        }

        static int CompareNPotScale(string nPotScale1, string nPotScale2)
        {
            var nPotScale1IsNoneNot = nPotScale1 == BuildReportRemastered.TextureData.NPOT_SCALE_NONE_NOT_POT;
            var nPotScale2IsNoneNot = nPotScale1 == BuildReportRemastered.TextureData.NPOT_SCALE_NONE_NOT_POT;

            if (nPotScale1IsNoneNot && !nPotScale2IsNoneNot)
            {
                return -1;
            }
            if (!nPotScale1IsNoneNot && nPotScale2IsNoneNot)
            {
                return 1;
            }

            return string.Compare(nPotScale1, nPotScale2, StringComparison.Ordinal);
        }

        // =============================================================================================================

        static void SortByInt(BuildReportRemastered.SizePart[] assetList, BuildReportRemastered.AssetList.SortOrder sortOrder)
        {
            if (sortOrder == BuildReportRemastered.AssetList.SortOrder.Descending)
            {
                Array.Sort(assetList, delegate (BuildReportRemastered.SizePart entry1, BuildReportRemastered.SizePart entry2)
                {
                    int sortResult = entry2.GetIntAuxData().CompareTo(entry1.GetIntAuxData());
                    if (sortResult != 0)
                    {
                        return sortResult;
                    }

                    // same texture data
                    // sort by asset size for assets with texture data
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
                    int sortResult = entry1.GetIntAuxData().CompareTo(entry2.GetIntAuxData());
                    if (sortResult != 0)
                    {
                        return sortResult;
                    }

                    // same texture data
                    // sort by asset size for assets with same texture data
                    if (entry1.UsableSize > entry2.UsableSize) return 1;
                    if (entry1.UsableSize < entry2.UsableSize) return -1;

                    // same size
                    // sort by asset name for assets with same sizes
                    return SortByAssetNameAscending(entry1, entry2);
                });
            }
        }

        static void SortByFloat(BuildReportRemastered.SizePart[] assetList, BuildReportRemastered.AssetList.SortOrder sortOrder)
        {
            if (sortOrder == BuildReportRemastered.AssetList.SortOrder.Descending)
            {
                Array.Sort(assetList, delegate (BuildReportRemastered.SizePart entry1, BuildReportRemastered.SizePart entry2)
                {
                    int sortResult = entry1.GetFloatAuxData().CompareTo(entry2.GetFloatAuxData());
                    if (sortResult != 0)
                    {
                        return sortResult;
                    }

                    // same texture data
                    // sort by asset size for assets with texture data
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
                    int sortResult = entry2.GetFloatAuxData().CompareTo(entry1.GetFloatAuxData());
                    if (sortResult != 0)
                    {
                        return sortResult;
                    }

                    // same texture data
                    // sort by asset size for assets with same texture data
                    if (entry1.UsableSize > entry2.UsableSize) return 1;
                    if (entry1.UsableSize < entry2.UsableSize) return -1;

                    // same size
                    // sort by asset name for assets with same sizes
                    return SortByAssetNameAscending(entry1, entry2);
                });
            }
        }

        static void SortByText(BuildReportRemastered.SizePart[] assetList, BuildReportRemastered.AssetList.SortOrder sortOrder)
        {
            if (sortOrder == BuildReportRemastered.AssetList.SortOrder.Descending)
            {
                Array.Sort(assetList, delegate (BuildReportRemastered.SizePart entry1, BuildReportRemastered.SizePart entry2)
                {
                    int sortTextureTypeResult = string.Compare(entry1.GetTextAuxData(), entry2.GetTextAuxData(), StringComparison.OrdinalIgnoreCase);
                    if (sortTextureTypeResult != 0)
                    {
                        return sortTextureTypeResult;
                    }

                    // same texture type
                    // sort by asset size for assets with same texture types
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
                    int sortTextureTypeResult = string.Compare(entry2.GetTextAuxData(), entry1.GetTextAuxData(), StringComparison.OrdinalIgnoreCase);
                    if (sortTextureTypeResult != 0)
                    {
                        return sortTextureTypeResult;
                    }

                    // same texture type
                    // sort by asset size for assets with same texture types
                    if (entry1.UsableSize > entry2.UsableSize) return 1;
                    if (entry1.UsableSize < entry2.UsableSize) return -1;

                    // same size
                    // sort by asset name for assets with same sizes
                    return SortByAssetNameAscending(entry1, entry2);
                });
            }
        }

        static void SortByText(BuildReportRemastered.SizePart[] assetList, BuildReportRemastered.AssetList.SortOrder sortOrder, Func<string, string, int> compare)
        {
            if (sortOrder == BuildReportRemastered.AssetList.SortOrder.Descending)
            {
                Array.Sort(assetList, delegate (BuildReportRemastered.SizePart entry1, BuildReportRemastered.SizePart entry2)
                {
                    int sortTextureTypeResult = compare(entry1.GetTextAuxData(), entry2.GetTextAuxData());
                    if (sortTextureTypeResult != 0)
                    {
                        return sortTextureTypeResult;
                    }

                    // same texture type
                    // sort by asset size for assets with same texture types
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
                    int sortTextureTypeResult = compare(entry2.GetTextAuxData(), entry1.GetTextAuxData());
                    if (sortTextureTypeResult != 0)
                    {
                        return sortTextureTypeResult;
                    }

                    // same texture type
                    // sort by asset size for assets with same texture types
                    if (entry1.UsableSize > entry2.UsableSize) return 1;
                    if (entry1.UsableSize < entry2.UsableSize) return -1;

                    // same size
                    // sort by asset name for assets with same sizes
                    return SortByAssetNameAscending(entry1, entry2);
                });
            }
        }

        // =============================================================================================================

        static void SortTextureData(BuildReportRemastered.SizePart[] assetList, BuildReportRemastered.TextureData textureData, BuildReportRemastered.AssetList.SortOrder sortOrder, Func<BuildReportRemastered.TextureData.Entry, bool> func)
        {
            var textureEntries = textureData.GetTextureData();

            for (int n = 0; n < assetList.Length; ++n)
            {
                int boolValue = 0;
                if (textureEntries.ContainsKey(assetList[n].Name))
                {
                    boolValue = func(textureEntries[assetList[n].Name]) ? 1 : 0;
                }

                assetList[n].SetIntAuxData(boolValue);
            }

            SortByInt(assetList, sortOrder);
        }

        static void SortTextureData(BuildReportRemastered.SizePart[] assetList, BuildReportRemastered.TextureData textureData, BuildReportRemastered.AssetList.SortOrder sortOrder, Func<BuildReportRemastered.TextureData.Entry, string> func)
        {
            var textureEntries = textureData.GetTextureData();

            for (int n = 0; n < assetList.Length; ++n)
            {
                string textData = null;
                if (textureEntries.ContainsKey(assetList[n].Name))
                {
                    textData = func(textureEntries[assetList[n].Name]);
                }

                assetList[n].SetTextAuxData(textData);
            }

            SortByText(assetList, sortOrder);
        }

        static void SortTextureData(BuildReportRemastered.SizePart[] assetList, BuildReportRemastered.TextureData textureData, BuildReportRemastered.AssetList.SortOrder sortOrder, Func<BuildReportRemastered.TextureData.Entry, float> func)
        {
            var textureEntries = textureData.GetTextureData();

            for (int n = 0; n < assetList.Length; ++n)
            {
                float floatValue = 0;
                if (textureEntries.ContainsKey(assetList[n].Name))
                {
                    floatValue = func(textureEntries[assetList[n].Name]);
                }

                assetList[n].SetFloatAuxData(floatValue);
            }

            SortByFloat(assetList, sortOrder);
        }

        static void SortTextureData(BuildReportRemastered.SizePart[] assetList, BuildReportRemastered.TextureData textureData, BuildReportRemastered.AssetList.SortOrder sortOrder, Func<BuildReportRemastered.TextureData.Entry, int> func)
        {
            var textureEntries = textureData.GetTextureData();

            for (int n = 0; n < assetList.Length; ++n)
            {
                int intValue = 0;
                if (textureEntries.ContainsKey(assetList[n].Name))
                {
                    intValue = func(textureEntries[assetList[n].Name]);
                }

                assetList[n].SetIntAuxData(intValue);
            }

            SortByInt(assetList, sortOrder);
        }

        // NPotScale sort is special: we want the "None (Not Power of 2)" values to go at top, ignoring alphabetical order for that special value
        static void SortNPotScale(BuildReportRemastered.SizePart[] assetList, BuildReportRemastered.TextureData textureData, BuildReportRemastered.AssetList.SortOrder sortOrder)
        {
            var textureEntries = textureData.GetTextureData();

            for (int n = 0; n < assetList.Length; ++n)
            {
                string textData = null;
                if (textureEntries.ContainsKey(assetList[n].Name))
                {
                    textData = textureEntries[assetList[n].Name].NPotScale;
                }

                assetList[n].SetTextAuxData(textData);
            }

            if (sortOrder == BuildReportRemastered.AssetList.SortOrder.Descending)
            {
                Array.Sort(assetList, delegate (BuildReportRemastered.SizePart entry1, BuildReportRemastered.SizePart entry2)
                {
                    string nPotScale1 = entry1.GetTextAuxData();
                    string nPotScale2 = entry2.GetTextAuxData();

                    // put non-power-of-2 at top
                    bool nPotScale1IsNoneNot = nPotScale1 == BuildReportRemastered.TextureData.NPOT_SCALE_NONE_NOT_POT;
                    bool nPotScale2IsNoneNot = nPotScale2 == BuildReportRemastered.TextureData.NPOT_SCALE_NONE_NOT_POT;
                    if (nPotScale1IsNoneNot && !nPotScale2IsNoneNot)
                    {
                        return -1;
                    }
                    if (!nPotScale1IsNoneNot && nPotScale2IsNoneNot)
                    {
                        return 1;
                    }

                    // at this point, entry1 and entry 2 are not non-power-of-2 (or both of them are), so compare them as usual
                    int sortTextureTypeResult = string.Compare(nPotScale1, nPotScale2, StringComparison.OrdinalIgnoreCase);
                    if (sortTextureTypeResult != 0)
                    {
                        return sortTextureTypeResult;
                    }

                    // same nPotScale type
                    // sort by asset size for assets with same nPotScale types
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
                    string nPotScale1 = entry1.GetTextAuxData();
                    string nPotScale2 = entry2.GetTextAuxData();

                    // put non-power-of-2 at bottom
                    bool nPotScale1IsNoneNot = nPotScale1 == BuildReportRemastered.TextureData.NPOT_SCALE_NONE_NOT_POT;
                    bool nPotScale2IsNoneNot = nPotScale2 == BuildReportRemastered.TextureData.NPOT_SCALE_NONE_NOT_POT;
                    if (nPotScale1IsNoneNot && !nPotScale2IsNoneNot)
                    {
                        return 1;
                    }
                    if (!nPotScale1IsNoneNot && nPotScale2IsNoneNot)
                    {
                        return -1;
                    }

                    // at this point, entry1 and entry 2 are not non-power-of-2 (or both of them are), so compare them as usual
                    int sortTextureTypeResult = string.Compare(nPotScale2, nPotScale1, StringComparison.OrdinalIgnoreCase);
                    if (sortTextureTypeResult != 0)
                    {
                        return sortTextureTypeResult;
                    }

                    // same nPotScale type
                    // sort by asset size for assets with same nPotScale types
                    if (entry1.UsableSize > entry2.UsableSize) return 1;
                    if (entry1.UsableSize < entry2.UsableSize) return -1;

                    // same size
                    // sort by asset name for assets with same sizes
                    return SortByAssetNameAscending(entry1, entry2);
                });
            }
        }
    }
}