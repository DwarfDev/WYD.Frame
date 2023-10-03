namespace WYD.Frame.Models.Models;

public class AreaSize
{
    public int Height { get; set; }
    public int Width { get; set; }

    public AreaSize()
    {
        
        
    }
    public AreaSize(int width, int height)
    {
        Width = width;
        Height = height;
    }
}