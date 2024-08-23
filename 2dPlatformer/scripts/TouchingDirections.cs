using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Uses the Collider to check directions to see if the object is currently on the ground, touching the wall, pr touching the ceiling
public class TouchingDirections : MonoBehaviour
{
        public ContactFilter2D castFilter;
        public float groundDistance = 0.05f;
        public float wallDistance = 0.2f;
        public float ceilingDistance = 0.05f;
        CapsuleCollider2D touchingCol;
        Animator animator;
        RaycastHit2D[] groundHits = new RaycastHit2D[5];
        RaycastHit2D[] wallHits = new RaycastHit2D[5];
        RaycastHit2D[] ceilingHits = new RaycastHit2D[5];


          [SerializeField]
        private bool _isGrounded;

    public bool isGrounded { get {
        return _isGrounded;
    } private set
    {
        _isGrounded = value;
        animator.SetBool(AnimationStrings.isGrounded, value);

    }}

        [SerializeField]
        private bool _isOnWall;

    public bool isOnWall { get {
        return _isOnWall;
    } private set
    {
        _isOnWall = value;
        animator.SetBool(AnimationStrings.isOnWall, value);

    }}

     [SerializeField]
        private bool _isOnCeiling;
    private Vector2 wallCheckDirecton => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;

    public bool isOnCeiling { get {
        return _isOnCeiling;
    } private set
    {
        _isOnCeiling = value;
        animator.SetBool(AnimationStrings.isOnCeiling, value);

    }}

   

    private void Awake()
        {
           touchingCol = GetComponent<CapsuleCollider2D>();
           animator = GetComponent<Animator>();
        }

    // Update is called once per frame
    void FixedUpdate()
    {
        isGrounded = touchingCol.Cast(Vector2.down, castFilter, groundHits, groundDistance) > 0;
        isOnWall = touchingCol.Cast(wallCheckDirecton, castFilter, wallHits, wallDistance) > 0;
        isOnCeiling = touchingCol.Cast(Vector2.up, castFilter, ceilingHits, ceilingDistance) > 0;
    }
}
