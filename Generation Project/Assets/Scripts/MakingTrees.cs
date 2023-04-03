using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MakingTrees : MonoBehaviour
{
    public GameObject Tree;
    public GameObject BranchObj;
    public GameObject LeafObj;

    public int whenToMakeLeafAmt = 0;
    public int branchesMade = 0;
    public int branchesToMake = 2;
    public int iterations = 4;

    public int minAngle = 0;
    public int maxAngle = 0;

    public TMP_Dropdown dropdown;
    public TMP_InputField MinText;
    public TMP_InputField MaxText;
    public TMP_InputField BranchesToMake;
    public TMP_InputField Iterations;


    public Queue<Branch> lSystem(Branch root, Rule[] rules)
    {
        Debug.Log($"root {root.length}");

        Queue<Branch> tree = new Queue<Branch>();
        Queue<Branch> active = new Queue<Branch>();

        tree.Enqueue(root);
        active.Enqueue(root);

        whenToMakeLeafAmt = 0;
        int count = iterations;
        whenToMakeLeafAmt += count;


        while (count >= 0)
        {
            count--;
            Branch current = active.Dequeue();
            Debug.Log($"curr {current.length}");
            tree.Enqueue(current);

            foreach (Rule r in rules)
            {
                if (r.Matches(current))
                {   
                    
                    Branch[] results = r.RHS(current);
                    foreach (Branch b in results)
                    {
                        active.Enqueue(b);
                    }
                    break;
                }
            }

        }
        return tree;
    }
    // Start is called before the first frame update
    public void MakeTree()
    {
        Branch[] branches = FindObjectsOfType<Branch>();
        foreach(Branch b in branches)
        {
            Destroy(b.gameObject);
        }
        branchesMade = 0;
        int.TryParse(MinText.text, out minAngle);
        int.TryParse(MaxText.text, out maxAngle);
        int.TryParse(BranchesToMake.text, out branchesToMake);
        int.TryParse(Iterations.text, out iterations);


        Branch start = MakeMeABranch(Vector2.zero, 0, 1);
        Rule1 rule1 = new Rule1();
        Rule2 rule2 = new Rule2();

        Rule[] rules = new Rule[] { rule1 };

        if (dropdown.value == 0)
        {
            rules = new Rule[] { rule1 };
        }
        else if (dropdown.value == 1)
        {

            rules = new Rule[] { rule2 };
        }


        Queue<Branch> tree = lSystem(start, rules);
        //int i = 0;
        //foreach(Branch b in tree)
        //{
        //    i++;
        //    StartCoroutine(WaitForLine(b, i));
        //}

    }

    public Branch MakeMeABranch(Vector2 position, float orientation, float length)
    {

        GameObject treePart = Instantiate(Resources.Load("Branch") as GameObject, GameObject.Find("GameManager").transform, false);
        Branch b = treePart.GetComponent<Branch>();
        b.startPosition = position;
        b.orientation = orientation;
        b.length = length;
        b.SetUpBranch(orientation, length);
        b.endPosition = b.gameObject.transform.position;
        treePart.GetComponent<LineRenderer>().SetPosition(0, b.startPosition);
        treePart.GetComponent<LineRenderer>().SetPosition(1, b.endPosition);
        if(branchesMade > whenToMakeLeafAmt)
        {
            treePart.GetComponent<LineRenderer>().startColor = Color.green;
            treePart.GetComponent<LineRenderer>().endColor = Color.green;
        }
        else
        {
            treePart.GetComponent<LineRenderer>().startColor = Color.white;
            treePart.GetComponent<LineRenderer>().endColor = Color.white;
        }
        branchesMade++;

        return b;
    }
    IEnumerator WaitForLine(Branch b, int delay)
    {
        yield return new WaitForSeconds(delay + 3);

        //Debug.DrawLine(b.startPosition, b.endPosition, Color.green, 100f);
        //GameObject treePart = Instantiate(BranchObj, transform, false);
        //treePart.GetComponent<Branch>().SetUpBranch(b.orientation, b.length);
        //treePart.GetComponent<LineRenderer>().SetPosition(0, b.startPosition);
        //treePart.GetComponent<LineRenderer>().SetPosition(1, b.endPosition);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

//public class Branch : MonoBehaviour
//{
//    public Vector2 startPosition;
//    public Vector2 endPosition;
//    public float orientation;
//    public float length;

//    //public Branch(Vector2 position, float orientation, float length) 
//    //{
//    //    this.startPosition = position;
//    //    this.orientation = (orientation*-90);
//    //    this.length = length;
//    //    Debug.Log("aa" + this.orientation);
//    //    float y = length * Mathf.Sin(this.orientation * 2 * Mathf.PI);

//    //    float x = length * Mathf.Cos(this.orientation * 2 * Mathf.PI);
//    //    this.endPosition = new Vector2(x, y);
//    //    Debug.Log($"X: {endPosition.x}, Y: {endPosition.y}, Ori: {this.orientation}, L: {this.length}");

//    //}

//}

public class Rule : MonoBehaviour
{
    public MakingTrees mk;
    public virtual bool Matches(Branch branch) { return true; }
    public virtual Branch[] RHS(Branch branch) { return new Branch[] { }; }
}

public class Rule1 : Rule
{
    
    public override bool Matches(Branch branch) { return true; }
    public override Branch[] RHS(Branch branch) 
    {
        mk = FindObjectOfType<MakingTrees>();
        List<Branch> newBranches = new List<Branch>();

        for(int i = 0; i < mk.branchesToMake; i++)
        {
            
            Branch temp = mk.MakeMeABranch(branch.endPosition, branch.orientation + Random.Range(mk.minAngle, mk.maxAngle), branch.length * 2 / 3);
            newBranches.Add(temp);
        }

        //Branch b = mk.MakeMeABranch(branch.endPosition, branch.orientation + 45, branch.length * 2 / 3);
        
        return newBranches.ToArray();
    }
}

public class Rule2 : Rule
{
    public override bool Matches(Branch branch) { return true; }
    public override Branch[] RHS(Branch branch)
    {
        mk = FindObjectOfType<MakingTrees>();
        Branch a = mk.MakeMeABranch(branch.endPosition, branch.orientation - mk.minAngle, branch.length * 2 / 3);
        Branch b = mk.MakeMeABranch(branch.endPosition, branch.orientation + mk.minAngle, branch.length / 2);
        return new Branch[] { a, b};
    }
}
