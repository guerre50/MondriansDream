using UnityEngine;
using System.Collections;

public static class Utils {
	
	
	public static bool LineIntersection(Bounds horizontalLine, Bounds verticalLine, ref Vector2 intersection) {
		Vector3 line1A = horizontalLine.center - Vector3.right*horizontalLine.extents.x*100;
		Vector3 line1B = horizontalLine.center + Vector3.right*horizontalLine.extents.x*100;
		Vector3 line2A = verticalLine.center - Vector3.forward*verticalLine.extents.z*100;
		Vector3 line2B = verticalLine.center + Vector3.forward*verticalLine.extents.z*100;
		
		Vector2 inter = Vector2.zero;
		bool res = LineIntersection(new Vector2(line1A.x, line1A.z),
									new Vector2(line1B.x, line1B.z),
									new Vector2(line2A.x, line2A.z),
									new Vector2(line2B.x, line2B.z), ref inter);
		intersection = inter;
		return res;
	}
	
	public static Vector3 ToVector3(Color color) {
		return new Vector3(color.r, color.g, color.b);
	}
	
	
	
	public static bool LineIntersection( Vector2 p1,Vector2 p2, Vector2 p3, Vector2 p4, ref Vector2 intersection ){
		// http://www.stefanbader.ch/faster-line-segment-intersection-for-unity3dc/
	    float Ax,Bx,Cx,Ay,By,Cy,d,e,f,num/*,offset*/;
	    float x1lo,x1hi,y1lo,y1hi;
	
	    Ax = p2.x-p1.x;
	    Bx = p3.x-p4.x;
		
	    // X bound box test/
	    if(Ax<0) {
	        x1lo=p2.x; x1hi=p1.x;
	    } else {
	        x1hi=p2.x; x1lo=p1.x;
	    }
	
	    if(Bx>0) {
	        if(x1hi < p4.x || p3.x < x1lo) return false;
	    } else {
	        if(x1hi < p3.x || p4.x < x1lo) return false;
	    }
		
	    Ay = p2.y-p1.y;
	    By = p3.y-p4.y;
	
	    // Y bound box test//
	    if(Ay<0) {     
	        y1lo=p2.y; y1hi=p1.y;
	    } else {
	        y1hi=p2.y; y1lo=p1.y;
	    }
	
	    if(By>0) {
	        if(y1hi < p4.y || p3.y < y1lo) return false;
	    } else {
	        if(y1hi < p3.y || p4.y < y1lo) return false;
	    }
	
	    Cx = p1.x-p3.x;
	    Cy = p1.y-p3.y;
	    d = By*Cx - Bx*Cy;  // alpha numerator//
	    f = Ay*Bx - Ax*By;  // both denominator//
	
	
	    // alpha tests//
	    if(f>0) {
	        if(d<0 || d>f) return false;
	    } else {
	        if(d>0 || d<f) return false;
	    }
	
	    e = Ax*Cy - Ay*Cx;  // beta numerator//
	
	    // beta tests //
	    if(f>0) {      
	      	if(e<0 || e>f) return false;
        } else {
	      	if(e>0 || e<f) return false;
	    }
	 
	
	    // check if they are parallel
	    if(f==0) return false;
	        
	
	    // compute intersection coordinates //
	    num = d*Ax; // numerator //
	
	    intersection.x = p1.x + num / f;
		
	    num = d*Ay;
		
		intersection.y = p1.y + num / f;
    
		return true;
	}
}
