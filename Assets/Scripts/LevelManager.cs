using UnityEngine ;
using System.Collections.Generic ;

public class LevelManager : MonoBehaviour {
   [Header ("Level texture")]
   [SerializeField] private Texture2D levelTexture ;

   [Header ("Tiles Prefabs")]
   [SerializeField] private GameObject prefabWallTile ;
   [SerializeField] private GameObject prefabRoadTile ;

   [Header ("Ball and Road paint color")]
   public Color paintColor ;

   [HideInInspector] public List<RoadTile> roadTilesList = new List<RoadTile> () ;
   [HideInInspector] public RoadTile defaultBallRoadTile ;


   private Color colorWall = Color.white ;
   private Color colorRoad = Color.black ;

   private float unitPerPixel ;


   private void Awake () {
      Generate () ;
      //assign first road tile as default poition for the ball:
      defaultBallRoadTile = roadTilesList [ 0 ] ;
   }

   private void Generate () {
      unitPerPixel = prefabWallTile.transform.lossyScale.x ;
      float halfUnitPerPixel = unitPerPixel / 2f ;

      float width = levelTexture.width ;
      float height = levelTexture.height ;

      Vector3 offset = (new Vector3 (width / 2f, 0f, height / 2f) * unitPerPixel)
                       - new Vector3 (halfUnitPerPixel, 0f, halfUnitPerPixel) ;

      for (int x = 0; x < width; x++) {
         for (int y = 0; y < height; y++) {
            //Get pixel color :
            Color pixelColor = levelTexture.GetPixel (x, y) ;

            Vector3 spawnPos = ((new Vector3 (x, 0f, y) * unitPerPixel) - offset) ;

            if (pixelColor == colorWall)
               Spawn (prefabWallTile, spawnPos) ;
            else if (pixelColor == colorRoad)
               Spawn (prefabRoadTile, spawnPos) ;
         }
      }
   }

   private void Spawn (GameObject prefabTile, Vector3 position) {
      //fix Y position:
      position.y = prefabTile.transform.position.y ;

      GameObject obj = Instantiate (prefabTile, position, Quaternion.identity, transform) ;

      if (prefabTile == prefabRoadTile)
         roadTilesList.Add (obj.GetComponent <RoadTile> ()) ;
   }
}
