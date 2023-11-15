namespace ASFileExplorer;

public enum DisplaySchemas
{
    SIMPLEROW,
    IMAGEROW
}

public class CustomDataTemplateSelector : DataTemplateSelector
{
    private DisplaySchemas SelectedSchema;

    public DataTemplate DesktopTemplate { get; set; }
    public DataTemplate MobileTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        switch (SelectedSchema)
        {
            case DisplaySchemas.SIMPLEROW:
                return DesktopTemplate;
            case DisplaySchemas.IMAGEROW:
                return MobileTemplate;
        }
        return null;
    }

    public void ChangeSchema(DisplaySchemas schema)
    {
        this.SelectedSchema = schema;
    }

}


