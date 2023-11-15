namespace ASProject.Helpers;

public class ImageHelper
{
    public static byte[] GetImageStreamAsBytes(Stream input)
    {
        var buffer = new byte[16 * 1024];
        using (MemoryStream ms = new MemoryStream())
        {
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }
            return ms.ToArray();
        }
    }

    public static async Task<byte[]> ConvertImageSourceToBytes(ImageSource imageSource)
    {
        if (imageSource is StreamImageSource streamSource)
        {
            var stream = await streamSource.Stream.Invoke(CancellationToken.None);
            return GetImageStreamAsBytes(stream);
        }
        else
        {
            throw new Exception("0x13 -> ImageHelper.SquishImageSource");
        }
    }

}

