using System;

namespace BuildReportRemastered
{
    public static partial class AssetListUtility
    {
        public static void SortAssetList(BuildReportRemastered.SizePart[] assetList, BuildReportRemastered.MeshData meshData, MeshData.DataId meshSortType, BuildReportRemastered.AssetList.SortOrder sortOrder)
        {
            switch (meshSortType)
            {
                case BuildReportRemastered.MeshData.DataId.MeshFilterCount:
                    SortMeshData(assetList, meshData, sortOrder, entry => entry.MeshFilterCount);
                    break;
                case BuildReportRemastered.MeshData.DataId.SkinnedMeshRendererCount:
                    SortMeshData(assetList, meshData, sortOrder, entry => entry.SkinnedMeshRendererCount);
                    break;
                case BuildReportRemastered.MeshData.DataId.SubMeshCount:
                    SortMeshData(assetList, meshData, sortOrder, entry => entry.SubMeshCount);
                    break;
                case BuildReportRemastered.MeshData.DataId.VertexCount:
                    SortMeshData(assetList, meshData, sortOrder, entry => entry.VertexCount);
                    break;
                case BuildReportRemastered.MeshData.DataId.TriangleCount:
                    SortMeshData(assetList, meshData, sortOrder, entry => entry.TriangleCount);
                    break;
                case BuildReportRemastered.MeshData.DataId.AnimationType:
                    SortMeshData(assetList, meshData, sortOrder, entry => entry.AnimationType);
                    break;
                case BuildReportRemastered.MeshData.DataId.AnimationClipCount:
                    SortMeshData(assetList, meshData, sortOrder, entry => entry.AnimationClipCount);
                    break;
            }
        }

        static void SortMeshData(BuildReportRemastered.SizePart[] assetList, BuildReportRemastered.MeshData meshData, BuildReportRemastered.AssetList.SortOrder sortOrder, Func<BuildReportRemastered.MeshData.Entry, int> func)
        {
            var meshEntries = meshData.GetMeshData();

            for (int n = 0; n < assetList.Length; ++n)
            {
                int intValue = 0;
                if (meshEntries.ContainsKey(assetList[n].Name))
                {
                    intValue = func(meshEntries[assetList[n].Name]);
                }

                assetList[n].SetIntAuxData(intValue);
            }

            SortByInt(assetList, sortOrder);
        }

        static void SortMeshData(BuildReportRemastered.SizePart[] assetList, BuildReportRemastered.MeshData meshData, BuildReportRemastered.AssetList.SortOrder sortOrder, Func<BuildReportRemastered.MeshData.Entry, string> func)
        {
            var meshEntries = meshData.GetMeshData();

            for (int n = 0; n < assetList.Length; ++n)
            {
                string textData = null;
                if (meshEntries.ContainsKey(assetList[n].Name))
                {
                    textData = func(meshEntries[assetList[n].Name]);
                }

                assetList[n].SetTextAuxData(textData);
            }

            SortByText(assetList, sortOrder);
        }
    }
}