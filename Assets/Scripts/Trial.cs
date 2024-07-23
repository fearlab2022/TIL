public class Trial
{
    public bool predRender { get; private set; }
    public bool cageRender { get; private set; }
    public bool predChase { get; private set; }
    public bool isGreen { get; private set; }
    public bool renderMiddleScreen { get; private set; }
    public int middleScreenType { get; private set; } // 5 for fixation, 6 for question

    public Trial(bool predRender, bool cageRender, bool predChase, bool isGreen, bool renderMiddleScreen, int middleScreenType)
    {
        this.predRender = predRender;
        this.cageRender = cageRender;
        this.predChase = predChase;
        this.isGreen = isGreen;
        this.renderMiddleScreen = renderMiddleScreen;
        this.middleScreenType = middleScreenType;
    }

    public override string ToString()
    {
        return $"PredRender: {predRender}, CageRender: {cageRender}, PredChase: {predChase}, IsGreen: {isGreen}, RenderMiddleScreen: {renderMiddleScreen}, MiddleScreenType: {middleScreenType}";
    }
}
