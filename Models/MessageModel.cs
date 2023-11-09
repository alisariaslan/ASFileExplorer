
namespace ASFileExplorer;

public enum MessageType
{
	REQUEST_ALL_PERMS, //Low level
	CHECK_ONE_PERM, //Low level
	PERM_IS_CHECKED //High Level
}

public enum PermType
{
	READ_EXTERNAL_STORAGE,
	WRITE_EXTERNAL_STORAGE,
	MANAGE_EXTERNAL_STORAGE, //30
	READ_MEDIA_IMAGES, //33
	READ_MEDIA_VIDEO, //33
	READ_MEDIA_AUDIO, //33
	REQUEST_INSTALL_PACKAGES, //23
	START_VIEW_PERMISSION_USAGE //30
}

public record MessageData(MessageType typeOfMessage, object data);

public class PermModel
{
	public PermType Perm { get; set; }
	public string Key { get; set; }
	public bool Permitted { get; set; }

	public PermModel(PermType perm)
	{
#if ANDROID
		Key = MainActivity.ManifestPerms[(int)perm];
#endif
		Perm = perm;
	}

	public PermModel(PermType perm, bool permitted)
	{
#if ANDROID
		Key = MainActivity.ManifestPerms[(int)perm];
#endif
		Perm = perm;
		Permitted = permitted;
	}

}