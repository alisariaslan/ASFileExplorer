namespace ASFileExplorer;

public enum BodyDisplayTemplates
{
	SIMPLEROW,
	IMAGEROW
}

public class BodyTemplateSelector : DataTemplateSelector
{

	public static BodyDisplayTemplates SelectedTemplate;
	public DataTemplate Template1 { get; set; } = new DataTemplate();
	public DataTemplate Template2 { get; set; } = new DataTemplate();

	protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
	{
		switch (SelectedTemplate)
		{
			case BodyDisplayTemplates.SIMPLEROW:
				return Template1;
			case BodyDisplayTemplates.IMAGEROW:
				return Template2;

			default:
				return Template1;
		}
	}



}


