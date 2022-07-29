using System.Collections;
using System.Collections.Generic;
/// Include the name space for TextMesh Pro
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EndoControl : MonoBehaviour
{
    // steo 0 is the welcome screen and empty table
    [Header("Organelles To Spawn")]
    public int _step = 0;

    private int _totalSteps = 19;

    public GameObject _quizPanel;
    public GameObject _protein;
    public GameObject _nucleolus;
    public GameObject _ribosome40;
    public GameObject _ribosome60;
    public GameObject _ribosomefull;
    public GameObject _mrna;
    public GameObject _glycoprotein;
    public GameObject _rER;
    public GameObject _vesicleEmpty;
    public GameObject _vesiclegp;
    public GameObject _golgi;
    public GameObject _endosome;
    public GameObject _lysosome;
    public GameObject _damagedMitochondria;

    // store the spawned objects in global variables
    private GameObject _proteinSpawned;
    private GameObject _nucleolusSpawned;
    private GameObject _ribosome40Spawned;
    private GameObject _ribosome60Spawned;
    private GameObject _ribosomefullSpawned;
    private GameObject _mrnaSpawned;
    private GameObject _glycoproteinSpawned;
    private GameObject _rERSpawned;
    private GameObject _vesicleEmptySpawned;
    private GameObject _vesiclegpSpawned;
    private GameObject _golgiSpawned;
    private GameObject _endosomeSpawned;
    private GameObject _lysosomeSpawned;
    private GameObject _damagedMitochondriaSpawn;

    // variable to track if the animation should play
    private bool _playAnimation;

    // UI global variables
    [Header("UI Variables")]
    public GameObject _textArea;
    public GameObject _backButton;
    public GameObject _nextButton;
    public GameObject _nextButtonText;


    // UI panel text variables
    private List<string> endoUIStrings = new List<string>() {
        //0 > Start button
        "Welcome to the Cell Tour! Whenever you are ready, press the start button below to begin.",

        //1 > Highlight cytoplasm
        "We begin our tour in the cytoplasm, the space in-between the nucleus and the cell membrane. " +
        "All of the cell's components that are not in the nucleus reside here, suspended in fluid " +
        "called cytosol. The endomembrane system consists of all the cell's components that are " +
        "divided by membranes into specialized functional units called organelles. " +
        "Over the course of the tour, we will be creating proteins that will be shipped outside of the cell.",

        //2 > Highlight Nucleus
        "Our first stop on the tour of these organelles is the nucleus. The nucleus is the " +
        "most conspicuous organelle distinguishable in an animal cell. It contains almost all " +
        "of the cell's DNA, with which it can create ribosomes and proteins needed for cell function. " +
        "It is enclosed within the nuclear envelope, a double membrane, with each membrane composed of " +
        "a lipid bilayer. Scattered along the surface of the nucleus are nuclear pores, complexes of " +
        "proteins that regulate what can go in and out of the nucleus.",

        //3 > Spawn protein
        "Your first task is to create a ribosome, which is the component responsible for reading " +
        "genetic information in the form of mRNA and piecing together the protein for which it codes.\n" +
        "Collect a protein that will be used to create the ribosomal subunits.",

        //4 > Highlight chromatin
        "Within the nucleus, DNA is wrapped into discrete units called chromosomes. " +
        "Each chromosome is composed of one long DNA molecule and proteins that assist in coiling, " +
        "expressing or repressing certain areas that individual cells need for their function or stage in life. " +
        "Such a complex of proteins and DNA is called the chromatin.",

        //5 > Highlight nucleolus > spawn nucleolus > player gives nucleolus protein
        "Additionally within the nucleus resides a large structure called the nucleolus, where rRNA " +
        "and a bunch of proteins are arranged to form two different sizes of subunits that make up a complete ribosome." +
        "Take the protein you grabbed earlier and feed it into the nucleolus to form the ribosome subunits.",

        //6 > Highlight endoplasmic reticulum
        "As we make our way outside of the nucleus, notice the largest network of membranes in the cell, the endoplasmic reticulum. " +
        "It is comprised of two distinct, yet connected regions that differ in function: the rough and the smooth endoplasmic reticulum. ",

        //7 > highlight RER
        "The rough endoplasmic reticulum is called so because it is covered on the outside in ribosomes, " +
        "lending a \"rough\" appearance. Its membrane, which separates the inside of the ER- called the " +
        "ER lumen- with the cytosol, is continuous with the nuclear envelope and is where secretory protein " + 
        "and cell membrane production primarily occurs.",
        
        //8 > highlight SER > player combines subunits
        "The smooth ER's surface lacks ribosomes, and houses enzymes used in the production of lipids " +
        "vital for organism function, as well as in detoxification of drugs and poisons. \n" +
        "Combine the two subunits together to form a ribosome.",

        //9 > highlight ribosomes > spawn mRNA
        "Great! Now that we have a ribosome, all we need is a strand of mRNA in order to create a protein. Ribosomes that are found " +
        "floating in the cytosol are called free ribosomes, and those attached to the rough ER and nuclear membrane are called bound " +
        "ribosomes. Most proteins made by free ribosomes function in the cytosol, while those made by bound ribosomes are usually " +
        "shipped to other organelles such as lysosomes, or to be exported out of the cell.\n" +
        "Pick up a strand of mRNA and feed it into the ribosome in order to create a protein.",

        //10 > spawn protein variant
        "When a ribosome comes into contact with mRNA, it begins the process of protein translation. " +
        "During translation, mRNA is read by the ribosome and decoded into a specific chain of amino acids, called a polypeptide." +
        "The polypeptide will then fold into a specific shape that will define its function as a protein.\n" +
        "Take the protein created by the ribosome.",

        //11 > spawn vesicle
        "Proteins that are created in the rough ER are shipped via membraned bubbles called transport vesicles from the rough ER " +
        "to the golgi apparatus. The vesicles bud off from a special region known as the transitional ER and are " +
        "then carried to their destination along a highway of tubes - called microtubules- via motor proteins.\n" +
        "Place the protein inside a transport vesicle.",

        //12 > highlight cytoskeleton
        "Microtubules are just one of a number of structures that make up the cytoskeleton, the network of fibers that provide support " +
        "for the cell and provide the means by which cells and their components move.",
        
        //13 > highlight microtubules
        "Microtubules are the highway of the cell, providing the means by which organelles move and get from one part of the cell " +
        "to the other via motor proteins. They also provide structure to the cell itself, resisting compressing forces " +
        "to keep the cell from being crushed. Special arrangements of microtubules make up cilia and flagella in eukaryotic cells. " +
        "Microfilaments, on the other hand, provide structure by bearing tension to keep the cell from being torn open. " +
        "They are what allows muscle cells to contract.",

        //14 > highlight filaments
        "Intermediate filaments - named so because they are in-between the sizes of microtubules and microfilaments - " +
        "are more permanent fixtures of cells that provide rigidity and help a cell retain its original shape. " + 
        "This is useful for maintaining the shape of macro structures in an organism, like cartilage, skin, nails, and hair.",

        //15 > highlight golgi > player places vesicle in golgi
        "The golgi apparatus functions like the postal service of the cell. This organelle, shaped like a stack of pancakes, " +
        "takes in products from the ER and processes them to ship to other components of the cell. " +
        "The stacks, called \"cisternae\", travel from one side of the golgi- the \"cis\" side- to the other - the \"trans\" side- " +
        "modifying their contents as they travel to prepare for delivery to their destination, whether it be another cell " +
        "component or outside as a secretory protein.\n" +
        "Place the vesicle in the cis side of the Golgi Apparatus.",

        //16 > play animation
        "Upon reaching the trans side of the golgi, the cisternae will break off into vesicles that will be carried to their " +
        "destination. If the cell is in charge of insulin production, for instance, the completed insulin will be carried " +
        "from the golgi to the cell membrane where the insulin will be released outside of the cell. Otherwise, various " +
        "markers placed on the vesicle and around the cell components allow the vesicle to be carried to the appropriate " +
        "cell component, such as a lysosome.\n" +
        "Collect the final product!",

        //17 > highlight vacuoles > spawn endosome > player fuses endosome and vesiclegp
        "The final two membraned organelles are the vacuoles and lysosomes. Vacuoles are membraned organelles that carry " +
        "out different functions in different kinds of cells. In plant cells, a massive vacuole occupies most of the space " +
        "of the cell. In animal cells, they can carry food and other enzymes into the cell that are needed. Some protists " +
        "use them to pump water outside of the cell to maintain equilibrium.\n" +
        "Last stop! Deliver the final product to a late endosome to create a lysosome.",

        //18 > highlight lysosome > spawn damaged mitochondria > player fuses lysosome and mito 
        "Lysosomes are the clean-up crew of the cell. They carry digestive enzymes that allow the cell to recycle damaged " +
        "organelles and digest macromolecules into nutrients for the cell to use. To digest the food incoming from vacuoles, " +
        "lysosomes will fuse with the vacuole, releasing its contents inside to digest whatever is inside. Lysosomes are formed " +
        "when endosomes-- an organelle associated with the trans Golgi network-- fuse with vesicles containing lysosomal enzymes," +
        " in the final part of a system called the endocytic pathway.\n" +
        "Use the lysosome to break down the damaged mitochondria.",

        //19 > player can start quiz
        "This brings us to the end of the tour! We hope you now have a better idea of the internal workings of the cell. " +
        "You may now take the After Action Review quiz to test your knowledge of the endomembrane system, or you can continue " +
        "to explore the different parts of the cell, including those not covered in this tour."
    };

    private string _startQuizText = "Quiz";

    // make the spawn locations global in case we need to change them
    private Vector3 _spawnLeft = new Vector3(1.5f, 1.2f, -2f);

    private Vector3 _spawnMiddle = new Vector3(0.0f, 1.2f, -2f);

    private Vector3 _spawnRight = new Vector3(-1.5f, 1.2f, -2f);

    void Start()
    {
        // put welcome text on the panel 0
        _textArea.GetComponent<TextMeshProUGUI>().text = endoUIStrings[0];

        // hide the back button
        _backButton.SetActive(false);

        // make the next button visible
        _nextButton.SetActive(true);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            prevStep();
        }
        if (Input.GetMouseButtonDown(1))
        {
            nextStep();
        }
    }

    void ClearEndoProcessObjects()
    {
        foreach (GameObject item in GameObject.FindGameObjectsWithTag("EndoProcess"))
        {
            Destroy(item);
        }
    }

    public void EnableNextButton()
    {
        _nextButton.SetActive(true);
    }

    // Decides what to do at each step
    public void Process(bool isForward)
    {
        //Debugging
        Debug.Log("Step " + _step);
        if(isForward) {
            Debug.Log("Forwards");
        } else {
            Debug.Log("Backwards");
        }

        //Display text for each step
        _textArea.GetComponent<TMPro.TextMeshProUGUI>().text = endoUIStrings[_step];

        //Depending on step, spawn or highlight objects
        switch (_step)
        {
            case 0: //welcome

                _backButton.SetActive(false);
                _nextButton.SetActive(true);
                break;

            case 1: //highlight cytoplasm

                // highlight cytoplasm

                _backButton.SetActive(true);
                _nextButton.SetActive(true);

                break;

            case 2: //highlight nucleus
                
                // highlight nucleus

                _backButton.SetActive(true);
                _nextButton.SetActive(true);

                break;

            case 3: //spawn protein
                ClearEndoProcessObjects();

                _proteinSpawned =
                    Instantiate(_protein, _spawnLeft, Quaternion.identity);

                _backButton.SetActive(true);
                _nextButton.SetActive(false);

                break;

            case 4: //Highlight chromatin

                //highlight chromatin

                _backButton.SetActive(true);
                _nextButton.SetActive(true);
                break;

            case 5: //highlight nucleolus > spawn nucleolus > player feeds protein to nucleolus

                if (!isForward) {
                    ClearEndoProcessObjects();
                    _proteinSpawned =
                        Instantiate(_protein, _spawnLeft, Quaternion.identity);
                }

                //highlight nucleolus

                //spawn nucleolus for player to give protein
                _nucleolusSpawned =
                    Instantiate(_nucleolus, _spawnMiddle, Quaternion.identity);

                //wait for player to combine nucleolus and protein
                _nextButton.SetActive(false);

                break;

            case 6: //highlight ER

                //highlight ER

                _backButton.SetActive(true);
                _nextButton.SetActive(true);

                break;

            case 7: //highlight RER

                //highlight RER

                _backButton.SetActive(true);
                _nextButton.SetActive(true);
                
                break;

            case 8: //highlight SER > player combines subunits
                ClearEndoProcessObjects();

                if (!isForward)
                {
                    ClearEndoProcessObjects();
                    //spawn ribosome subunits in case player does not have them
                    _ribosome40Spawned =
                        Instantiate(_ribosome40, _spawnLeft, Quaternion.identity);
                    _ribosome60Spawned =
                        Instantiate(_ribosome60, _spawnRight, Quaternion.identity);
                }

                //highlight SER

                //wait for player to combine ribosome subunits
                _nextButton.SetActive(false);

                break;
               
            case 9: //highlight ribosomes > spawn mRNA > player feeds mrna to ribosome

                if (!isForward) {
                    ClearEndoProcessObjects();
                    //spawn ribosome for player
                    _ribosomefullSpawned = 
                        Instantiate(_ribosomefull, _spawnRight, Quaternion.identity);
                }

                //highlight ribosomes
                
                _mrnaSpawned =
                    Instantiate(_mrna, _spawnLeft, Quaternion.identity);

                //wait for player to combine mRNA and ribosome
                _nextButton.SetActive(false);

                break;

            case 10: //spawn protein variant

                if (!isForward) {
                    ClearEndoProcessObjects();
                }

                _glycoproteinSpawned =
                    Instantiate(_glycoprotein,_spawnRight, Quaternion.identity);

                //wait for player to pick up protein
                _nextButton.SetActive(false);

                break;

            case 11: //spawn vesicle > player places protein in vesicle

                if (!isForward) {
                    ClearEndoProcessObjects();
                    _glycoproteinSpawned =
                        Instantiate(_glycoprotein,_spawnRight, Quaternion.identity);
                }

                _vesicleEmptySpawned =
                    Instantiate(_vesicleEmpty, _spawnMiddle, Quaternion.identity);

                //wait for player to place protein in vesicle
                _nextButton.SetActive(false);

                break;

            case 12: //highlight cytoskeleton

                //highlight cytoskeleton

                _nextButton.SetActive(true);
                _backButton.SetActive(true);

                break;

            case 13: //highlight microtubules

                //highlight microtubules

                _nextButton.SetActive(true);
                _backButton.SetActive(true);

                break;

            case 14: //highlight filaments

                //highlight filaments

                _backButton.SetActive(true);

                break;
            
            case 15: //highlight golgi > spawn golgi > player places vesicle in golgi

                if (!isForward) {
                    ClearEndoProcessObjects();

                    _playAnimation = false;

                    // spawn the vesicle glycoprotein
                    _vesiclegpSpawned =
                        Instantiate(_vesiclegp, _spawnRight, Quaternion.identity);
                }

                _golgiSpawned = Instantiate(_golgi, _spawnMiddle, Quaternion.identity);

                // Rotate the golgi
                _golgiSpawned.transform.Rotate(90.0f, 0.0f, 90.0f, Space.Self);

                _nextButton.SetActive(true);
 
                break;
            
            case 16: //play animation

                _golgiSpawned.GetComponent<Animation>().StartAnimation();
                // _playAnimation = true;
                // StartCoroutine(AnimationLoop());

                //highlight filaments

                //wait for player to collect vesicle
                _nextButton.SetActive(false);

                break;

            case 17: //highlight vacuoles

                //highlight vacuoles

                _nextButton.SetActive(true);
                _backButton.SetActive(true);
                break;
            
            case 18: //highlight filaments
                if (!isForward) {
                    //change next button back to next
                    _nextButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Next";
                } 

                //highlight filaments

                _nextButton.SetActive(true);
                _backButton.SetActive(true);
                break;

            case 19: //end tour

                // set next button text
                _nextButtonText.GetComponent<TMPro.TextMeshProUGUI>().text =
                    _startQuizText;

                break;

            default:
                Debug.Log("Can't move any more forward or backwards");
                break;
        }
    }

    // animationloop coroutine
    IEnumerator AnimationLoop()
    {
        while (_playAnimation == true)
        {
            // run the animation for testing purposes
            _golgiSpawned.GetComponent<Animation>().StartAnimation();
            yield return null;
        }
    }

    public void nextStep()
    {
        // if we press the next button while on the last steep then we want to start the quiz
        if (_step == _totalSteps)
        {
            _playAnimation = false;

            // Hide any organelles from the previous step
            ClearEndoProcessObjects();

            // Set the panel to active = false
            foreach (GameObject item in GameObject.FindGameObjectsWithTag("endoPanel"))
            {
                Destroy(item);
            }
            // set the panel active = true
            _quizPanel.SetActive(true);
            // Trigger the quiz from the quizTrigger component
            gameObject.GetComponent<QuizTrigger>().TriggerQuiz();

            Debug.Log("Start Quiz");
            return;
        }
        _step++;
        Process(true);
    }

    public void prevStep()
    {
        if (_step == 0) return;
        _step--;
        Process(false);
    }
}
