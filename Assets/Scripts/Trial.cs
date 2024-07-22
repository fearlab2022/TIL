public class Trial
{
    public bool predRender { get; private set; }
    public bool cageRender { get; private set; }
    public bool predChase { get; private set; }

    public Trial(bool predRender, bool cageRender, bool predChase)
    {
        this.predRender = predRender;
        this.cageRender = cageRender;
        this.predChase = predChase;
    }

    public override string ToString()
    {
        return $"PredRender: {predRender}, CageRender: {cageRender}, PredChase: {predChase}";
    }
}
