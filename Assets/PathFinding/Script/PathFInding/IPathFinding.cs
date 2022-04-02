using System.Collections.Generic;

public interface IPathFinding {
    public void Init(List<Node> nodeList);
    public void Find();
}