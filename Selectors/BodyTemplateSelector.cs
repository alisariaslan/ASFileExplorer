namespace ASFileExplorer;

public enum BodyDisplayTemplates
{
	SIMPLEROW,
	IMAGEROW_SMALL,
	IMAGEROW_BIG
}

public class BodyTemplateSelector : DataTemplateSelector
{
	public BodyDisplayTemplates SelectedTemplate;
	public DataTemplate Template1 { get; set; }
	public DataTemplate Template2 { get; set; }
    public DataTemplate Template3 { get; set; } 

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
	{
		switch (SelectedTemplate)
		{
			case BodyDisplayTemplates.SIMPLEROW:
				return Template1;
			case BodyDisplayTemplates.IMAGEROW_SMALL:
				return Template2;
            case BodyDisplayTemplates.IMAGEROW_BIG:
                return Template3;

            default:
				return Template1;
		}
	}



}


