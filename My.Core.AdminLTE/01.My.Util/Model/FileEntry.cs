namespace My.Util.Model
{
    /// <summary>
    /// 描述：文件信息
    /// 作者：wby 2019/10/25 15:32:27
    /// </summary>
    public struct FileEntry
    {
        public string FileName { get; set; }
        public byte[] FileBytes { get; set; }
        public FileEntry(string fileName,byte[] fileBytes)
        {
            FileName = fileName;
            FileBytes = fileBytes;
        }
    }
}
