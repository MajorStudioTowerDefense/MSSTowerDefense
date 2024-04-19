using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Flags]
public enum BotTags
{
    customer = 0x01,
    employee = 0x02,
    thief = 0x04,

}
public class Bot : MonoBehaviour
{
    //The name of the bot
    public string botName;

    //All bot has a budget, if the budget runs out, the bot will stop shopping
    public int botBudget;
    protected int _startBudget;

    //if You defined a shopping effect, it will be played when the bot is shopping
    public GameObject shoppingEffect;
    //if You defined a shopping sound, it will be played when the bot is shopping
    public AudioClip shoppingSFX;

    //A* pathfinding system for the bot
    //I did not figure out how to fully utilize it in script yet
    public AIPath aiPath;
    public AIDestinationSetter destinationSetter;

    //tag property
    [SerializeField]
    public BotTags tags = 0;

    //Item the customer is looking for
    public List<Items> needs = new List<Items>();
    public Items selectedItem;

    //UIs for the bot
    public TextMeshPro nameText;
    public TextMeshPro budgetText;
    public float nameTextOffsetX = 0f;
    public float nameTextOffsetY = 1f;
    
    /////////////////////////////////

    // These are convenient functions for adding/removing/checking for tags
    // Since they involve bitwise operations, probably best to use these functions instead of altering the tags property directly from code
    public bool hasTag(BotTags tag)
    {
        return (tags & tag) != 0;
    }

    public void removeTag(BotTags tagsToRemove)
    {
        tags = tags & ~(tagsToRemove);
    }

    public void addTag(BotTags tagsToAdd)
    {
        tags |= tagsToAdd;
    }

    /////////////////////////////////

    // Common Unity components that bots might have.
    // If the bot has the component when init is called, these variables will fill with the appropriate values.
    protected Rigidbody2D _body;
    public virtual Rigidbody2D body
    {
        get { return _body; }
    }
    protected SpriteRenderer _sprite;
    public virtual SpriteRenderer sprite
    {
        get { return _sprite; }
    }
    protected Animator _anim;
    protected Collider2D _collider;
    public virtual Collider2D mainCollider
    {
        get { return _collider; }
    }

    /////////////////////////////////

    // Here are convenient getters/shortcuts that let us grab and modify individual coordinates without
    // having to go through transform.position.

    public float globalX
    {
        get { return transform.position.x; }
        set { transform.position = new Vector3(value, transform.position.y, transform.position.z); }
    }

    public float globalY
    {
        get { return transform.position.y; }
        set { transform.position = new Vector3(transform.position.x, value, transform.position.z); }
    }

    public float localX
    {
        get { return transform.localPosition.x; }
        set { transform.localPosition = new Vector3(value, transform.localPosition.y, transform.localPosition.z); }
    }

    public float localY
    {
        get { return transform.localPosition.y; }
        set { transform.localPosition = new Vector3(transform.localPosition.x, value, transform.localPosition.z); }
    }

    /////////////////////////////////
    
    protected bool _isPurchasing;
    public bool isPurchasing
    {
        get { return _isPurchasing; }
        set { _isPurchasing = value; }
    }

    /////////////////////////////////

    /////////////////////////////////
    

    // Bot behavior functions defined here.

    // This function is explicitly called after a bot has been spawned and positioned.
    // We use this instead of the Unity-defined Start or Awake because we have extremely precise
    // control over when it's called (for instance, if we just instantiated a bot, Start won't be called until after our current code compeletes, etc.)
    public virtual void init()
    {
        _sprite = GetComponentInChildren<SpriteRenderer>();
        
        _anim = GetComponentInChildren<Animator>();
        _body = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _startBudget = botBudget;
        
        if (tags.HasFlag(BotTags.customer))
        {
            aiPath = GetComponent<AIPath>();
            destinationSetter = GetComponent<AIDestinationSetter>();
        }

        nameText = GetComponentInChildren<TextMeshPro>();
        botUIinit();
    }

    public virtual void botUIinit()
    {
        

    }

    public virtual void botUIUpdate()
    {
        

    }

    protected virtual void Update()
    {
        botUIUpdate();
    }
}
