using UnityEngine ;
using GG.Infrastructure.Utils.Swipe ;
using DG.Tweening ;
using System.Collections.Generic ;
using UnityEngine.Events ;

// added this line to sort tiles by distance from the ray's origin using LINQ Queries  ( Line 50 & 51 ):
using System.Linq ;

public class BallMovement : MonoBehaviour {
   [SerializeField] private SwipeListener swipeListener ;
   [SerializeField] private LevelManager levelManager ;

   [SerializeField] private float stepDuration = 0.1f ;
   [SerializeField] private LayerMask wallsAndRoadsLayer ;
   private const float MAX_RAY_DISTANCE = 10f ;

   public UnityAction<List<RoadTile>, float> onMoveStart ;

   private Vector3 moveDirection ;
   private bool canMove = true ;

   private void Start () {
      // change default ball position :
      transform.position = levelManager.defaultBallRoadTile.position ;

      swipeListener.OnSwipe.AddListener (swipe => {
         switch (swipe) {
            case "Right":
               moveDirection = Vector3.right ;
               break ;
            case "Left":
               moveDirection = Vector3.left ;
               break ;
            case "Up":
               moveDirection = Vector3.forward ;
               break ;
            case "Down":
               moveDirection = Vector3.back ;
               break ;
         }
         MoveBall () ;
      }) ;
   }

   private void MoveBall () {
      if (canMove) {
         canMove = false ;
         // add raycast in the swipe direction (from the ball) :
         RaycastHit[] hits = Physics.RaycastAll (transform.position, moveDirection, MAX_RAY_DISTANCE, wallsAndRoadsLayer.value)
                                    .OrderBy (hit => hit.distance).ToArray () ; // added this line to sort tiles by distance from the ray's origin

         Vector3 targetPosition = transform.position ;

         int steps = 0 ;

         List<RoadTile> pathRoadTiles = new List<RoadTile> () ;

         for (int i = 0; i < hits.Length; i++) {
            if (hits [ i ].collider.isTrigger) { // Road tile
               // add road tiles to the list to be painted:
               pathRoadTiles.Add (hits [ i ].transform.GetComponent <RoadTile> ()) ;
              
            } else { // Wall tile
               if (i == 0) { // means wall is near the ball
                  canMove = true ;
                  return ;
               }
               //else:
               steps = i ;
               targetPosition = hits [ i - 1 ].transform.position ;
               break ;
            }
         }

         //move the ball to targetPosition:
         float moveDuration = stepDuration * steps ;
         transform
            .DOMove (targetPosition, moveDuration)
            .SetEase (Ease.OutExpo)
            .OnComplete (() => canMove = true) ;

         if (onMoveStart != null)
            onMoveStart.Invoke (pathRoadTiles, moveDuration) ;
      }
   }
}
