public class Trial
{
    public bool predRender;
    public bool cageRender;
    public bool predChase;
    public bool isGreen;
    public bool EOB;
    public bool showQuestionScreen;
    public string questionText;
    public float startX;
    public float startY;

    public Trial(bool predRender, bool cageRender, bool predChase, bool isGreen, bool EOB, bool showQuestionScreen, string questionText, float startX, float startY)
    {
        this.predRender = predRender;
        this.cageRender = cageRender;
        this.predChase = predChase;
        this.isGreen = isGreen;
        this.EOB = EOB;
        this.showQuestionScreen = showQuestionScreen;
        this.questionText = questionText;
        this.startX = startX;
        this.startY = startY;
    }

    public Trial(bool predRender, bool cageRender, bool predChase, bool isGreen, bool EOB, bool showQuestionScreen, float startX, float startY)
    {
        this.predRender = predRender;
        this.cageRender = cageRender;
        this.predChase = predChase;
        this.isGreen = isGreen;
        this.EOB = EOB;
        this.showQuestionScreen = showQuestionScreen;
        this.startX = startX;
        this.startY = startY;
    }

    public override string ToString()
    {
        return $"PredRender: {predRender}, CageRender: {cageRender}, PredChase: {predChase}, IsGreen: {isGreen}, EOB: {EOB}, questionScreen: {showQuestionScreen}, startX: {startX}, startY: {startY}";
    }
}
