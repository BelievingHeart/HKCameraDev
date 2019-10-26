using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using HalconDotNet;
using ImageDebugger.Core.ImageProcessing.Utilts;
using ImageDebugger.Core.Models;
using MaterialDesignThemes.Wpf;

namespace ImageDebugger.Core.ImageProcessing.BottomView
{
    public partial class I94BottomViewMeasure
    {
           public async Task<ImageProcessingResult> ProcessAsync(List<HImage> images, FindLineConfigs findLineConfigs,
            ObservableCollection<FaiItem> faiItems, int indexToShow,
            SnackbarMessageQueue messageQueue)
        {
            #region Initial variables

            HObject imageUndistorted;
            HTuple changeOfBase;
            HTuple changeOfBaseInv;
            HTuple rotationMat;
            HTuple rotationMatInv;
            HTuple mapToWorld;
            HTuple mapToImage;
            HTuple xLeft, yLeft, xRight, yRight, xUp, yUp, xDown, yDown;
            HTuple baseLeftCol,
                baseLeftLen1,
                baseLeftLen2,
                baseLeftRadian,
                baseLeftRow,
                baseTopCol,
                baseTopLen1,
                baseTopLen2,
                baseTopRadian,
                baseTopRow,
                camParams;

            #endregion


            #region Change base

            var image = images[0];
            HalconScripts.GetI94BottomViewBaseRectsNoRectify(image,  _shapeModelHandle, out baseTopRow,
                out baseTopCol, out baseTopRadian, out baseTopLen1, out baseTopLen2, out baseLeftRow,
                out baseLeftCol, out baseLeftRadian, out baseLeftLen1, out baseLeftLen2, out mapToWorld, out mapToImage,
                out camParams
            );
//            images[0] = imageUndistorted.HobjectToHimage();


            var findLineManager = new FindLineManager(messageQueue);

            // Top base
            var findLineParamTop = findLineConfigs.FindLineParamsDict["TopBase"];
            var findLineFeedingsTop = findLineParamTop.ToFindLineFeeding();
            findLineFeedingsTop.Col = baseTopCol;
            findLineFeedingsTop.Row = baseTopRow;
            findLineFeedingsTop.Radian = baseTopRadian;
            findLineFeedingsTop.Len1 = baseTopLen1;
            findLineFeedingsTop.Len2 = baseTopLen2;
            findLineFeedingsTop.Transition = "negative";
            var lineTopBase = findLineManager.TryFindLine("X-aixs", image, findLineFeedingsTop);
            HalconScripts.SortLineLeftRight(lineTopBase.XStart, lineTopBase.YStart, lineTopBase.XEnd, lineTopBase.YEnd,
                out xLeft, out yLeft, out xRight, out yRight);

            // Right base
            var findLineParamRight = findLineConfigs.FindLineParamsDict["LeftBase"];
            var findLineFeedingsRight = findLineParamRight.ToFindLineFeeding();
            findLineFeedingsRight.Col = baseLeftCol;
            findLineFeedingsRight.Row = baseLeftRow;
            findLineFeedingsRight.Radian = baseLeftRadian;
            findLineFeedingsRight.Len1 = baseLeftLen1;
            findLineFeedingsRight.Len2 = baseLeftLen2;
            findLineFeedingsRight.Transition = "negative";
            var lineLeftBase = findLineManager.TryFindLine("Y-axis", image, findLineFeedingsRight);
            HalconScripts.SortLineUpDown(lineLeftBase.XStart, lineLeftBase.YStart, lineLeftBase.XEnd, lineLeftBase.YEnd,
                out xUp, out yUp, out xDown, out yDown);


            HalconScripts.GetChangeOfBase(xLeft, yLeft, xRight, yRight, xUp, yUp, xDown, yDown, out changeOfBase,
                out changeOfBaseInv, out rotationMat, out rotationMatInv);

            #endregion

            #region Find lines

            var coordinateSolver = new CoordinateSolver(changeOfBase, changeOfBaseInv, rotationMat, rotationMatInv,
                mapToWorld, mapToImage);

            // Update absolute find line locations
            findLineConfigs.GenerateLocationsAbs(coordinateSolver);
            // Find lines
            findLineManager.FindLineFeedings = findLineConfigs.GenerateFindLineFeedings();
            await findLineManager.FindLinesParallel(images);
            

            #endregion


            #region 21

            var lineF21Top = coordinateSolver.TranslateLineInWorldUnit(21, lineTopBase, true);
            var lineF21Bottom = coordinateSolver.TranslateLineInWorldUnit(30, lineTopBase, true);
            
            var p1F21 = findLineManager.GetLine("21left").Intersect(lineF21Top);
            var p2F21 = findLineManager.GetLine("21left").Intersect(lineF21Bottom);
            var p3F21 = findLineManager.GetLine("21right").Intersect(lineF21Top);
            var p4F21 = findLineManager.GetLine("21right").Intersect(lineF21Bottom);
            
            var valueF21Top = coordinateSolver.PointPointDistanceInWorld(p1F21, p3F21, true);
            var valueF21Bottom = coordinateSolver.PointPointDistanceInWorld(p2F21, p4F21, true);
            

            #endregion

            #region 23

            var lineF23Left = coordinateSolver.TranslateLineInWorldUnit(-5.5, lineLeftBase, true);
            var lineF23Right = coordinateSolver.TranslateLineInWorldUnit(-13.2, lineLeftBase, true);
            
            var lineF123Left = coordinateSolver.TranslateLineInWorldUnit(-5, lineLeftBase, true);
            var lineF123Center = coordinateSolver.TranslateLineInWorldUnit(-9.269, lineLeftBase, true);
            var lineF123Right = coordinateSolver.TranslateLineInWorldUnit(-14.5, lineLeftBase, true);
            
            var p1F23 = findLineManager.GetLine("23top").Intersect(lineF23Left);
            var p2F23 = findLineManager.GetLine("23top").Intersect(lineF23Right);
            var p3F23 = findLineManager.GetLine("23bottom").Intersect(lineF23Left);
            var p4F23 = findLineManager.GetLine("23bottom").Intersect(lineF23Right);
            
            var pLeftF23 = Point.CenterPointInImage(p1F23, p3F23);
            var pRightF23 = Point.CenterPointInImage(p2F23, p4F23);
            
            var valueF23Left = coordinateSolver.PointPointDistanceInWorld(p1F23, p3F23, true);
            var valueF23Right = coordinateSolver.PointPointDistanceInWorld(p2F23, p4F23, true);
            

            #endregion

            #region 24

            var lineF24Top = coordinateSolver.TranslateLineInWorldUnit(7.3, lineTopBase, true);
            var lineF24Bottom = coordinateSolver.TranslateLineInWorldUnit(8.5, lineTopBase, true);
            
            var p1F24 = findLineManager.GetLine("24.left").Intersect(lineF24Top);
            var p2F24 = findLineManager.GetLine("24.left").Intersect(lineF24Bottom);
            var p3F24 = findLineManager.GetLine("24.right").Intersect(lineF24Top);
            var p4F24 = findLineManager.GetLine("24.right").Intersect(lineF24Bottom);
            
            var pLeftF24 = Point.CenterPointInImage(p1F24, p2F24);
            var pRightF24 = Point.CenterPointInImage(p3F24, p4F24);
            
            var valueF24 = coordinateSolver.PointPointDistanceInWorld(pLeftF24, pRightF24, true);

            #endregion

            #region 25

            var rectCircleLeft =
                coordinateSolver.FindLineLocationRelativeToAbsolute(EdgeLocationsRelative["leftCircle"]);
            HTuple circleXLeft, circleYLeft, circleRadiusLeft;
            HObject leftCircleContour;
            HalconScripts.I94FindLeftCircle(image, out leftCircleContour, rectCircleLeft.Y, rectCircleLeft.X,
                MathUtils.ToRadian(rectCircleLeft.Angle), rectCircleLeft.Len1, rectCircleLeft.Len2, out circleXLeft,
                out circleYLeft, out circleRadiusLeft);
            
            var valueF25_1 = circleRadiusLeft.D * Weight * 2;
            var leftCenter = new Point(circleXLeft, circleYLeft);
            var distYCircleLeft = coordinateSolver.PointLineDistanceInWorld(leftCenter, lineTopBase);
            var distXCircleLeft = coordinateSolver.PointLineDistanceInWorld(leftCenter, lineLeftBase);
            var valueF25_2 = 2.0 * Point.Distance(new Point(distXCircleLeft, distYCircleLeft), new Point(9.299, 7.886));

            #endregion
            
            #region 26

            var lineF26Left =
                coordinateSolver.TranslateLineInWorldUnit(-0.6, findLineManager.GetLine("24.left").SortUpDown(), true);
            var lineF26Right =
                coordinateSolver.TranslateLineInWorldUnit(0.6, findLineManager.GetLine("24.right").SortUpDown(), true);
            
            var rectLeftTop = coordinateSolver.FindLineLocationRelativeToAbsolute(EdgeLocationsRelative["26-leftTop"]);
            var edgeLeftTop = GetContour(image, rectLeftTop);
            var pLeftTopF26 = LineContourIntersection(lineF26Left, edgeLeftTop);
            var rectLeftBottom =
                coordinateSolver.FindLineLocationRelativeToAbsolute(EdgeLocationsRelative["26-leftBottom"]);
            var edgeLeftBottom = GetContour(image, rectLeftBottom);
            var pLeftBottomF26 = LineContourIntersection(lineF26Left, edgeLeftBottom);
            var rectRightTop =
                coordinateSolver.FindLineLocationRelativeToAbsolute(EdgeLocationsRelative["26-rightTop"]);
            var edgeRightTop = GetContour(image, rectRightTop);
            var pRightTopF26 = LineContourIntersection(lineF26Right, edgeRightTop);
            var rectRightBottom =
                coordinateSolver.FindLineLocationRelativeToAbsolute(EdgeLocationsRelative["26-rightBottom"]);
            var edgeRightBottom = GetContour(image, rectRightBottom);
            var pRightBottomF26 = LineContourIntersection(lineF26Right, edgeRightBottom);
            
            var valueF26_1 = coordinateSolver.PointPointDistanceInWorld(pLeftTopF26, pLeftBottomF26, true);
            var valueF26_2 = coordinateSolver.PointPointDistanceInWorld(pRightTopF26, pRightBottomF26, true);
            

            #endregion

            #region 27

            var rectCircleRight =
                coordinateSolver.FindLineLocationRelativeToAbsolute(EdgeLocationsRelative["rightCircle"]);
            HTuple circleXRight, circleYRight, circleRadiusRight;
            HObject rightCircleContour;
            HalconScripts.I94FindRightCircle(image, out rightCircleContour, rectCircleRight.Y, rectCircleRight.X,
                MathUtils.ToRadian(rectCircleRight.Angle), rectCircleRight.Len1, rectCircleRight.Len2, out circleXRight,
                out circleYRight, out circleRadiusRight);
            
            var rightCenter = new Point(circleXRight, circleYRight);

            
            var lineF27Left = coordinateSolver.TranslateLineInWorldUnit(-23.5, lineLeftBase, true);
            var lineF27Right = coordinateSolver.TranslateLineInWorldUnit(-25, lineLeftBase, true);
            
            var pTopLeftF27 = findLineManager.GetLine("27.top").Intersect(lineF27Left);
            var pTopRightF27 = findLineManager.GetLine("27.top").Intersect(lineF27Right);
            var pBottomLeftF27 = findLineManager.GetLine("27.bottom").Intersect(lineF27Left);
            var pBottomRightF27 = findLineManager.GetLine("27.bottom").Intersect(lineF27Right);
            
            var pF27Top = Point.CenterPointInImage(pTopLeftF27, pTopRightF27);
            var pF27Bottom = Point.CenterPointInImage(pBottomLeftF27, pBottomRightF27);
            
            var value27_1 = coordinateSolver.PointPointDistanceInWorld(rightCenter, pF27Top, true);
            var value27_2 = coordinateSolver.PointPointDistanceInWorld(rightCenter, pF27Bottom, true);
            

            #endregion
            
            #region 28

            var valueF28_1 = circleRadiusRight.D * Weight * 2;
            var distYCircleRight = coordinateSolver.PointLineDistanceInWorld(rightCenter, lineTopBase);
            var distXCircleRight = coordinateSolver.PointLineDistanceInWorld(rightCenter, lineLeftBase);
            var valueF28_2 = 2.0 * Point.Distance(new Point(distXCircleRight, distYCircleRight), new Point(24.434, 16.624));  
            

            #endregion

            #region 29

            var lineF29Top = coordinateSolver.TranslateLineInWorldUnit(16, lineTopBase, true);
            var lineF29Bottom = coordinateSolver.TranslateLineInWorldUnit(17.2, lineTopBase, true);
            
            var pLeftTopF29 = findLineManager.GetLine("29.left").Intersect(lineF29Top);
            var pLeftBottomF29 = findLineManager.GetLine("29.left").Intersect(lineF29Bottom);
            var pRightTopF29 = findLineManager.GetLine("29.right").Intersect(lineF29Top);
            var pRightBottomF29 = findLineManager.GetLine("29.right").Intersect(lineF29Bottom);
            
            var pF29Left = Point.CenterPointInImage(pLeftTopF29, pLeftBottomF29);
            var pF29Right = Point.CenterPointInImage(pRightTopF29, pRightBottomF29);
            
            var valueF29_1 = coordinateSolver.PointPointDistanceInWorld(rightCenter, pF29Left, true);
            var valueF29_2 = coordinateSolver.PointPointDistanceInWorld(rightCenter, pF29Right, true);

            #endregion

            #region 31

            var line3F31 = findLineManager.GetLine("31.topLeft").SortUpDown();
            var line1F31 = findLineManager.GetLine("31.bottomRight").SortUpDown();
            var p1L5F31 = Point.CenterPointInImage(new Point(line3F31.XStart, line3F31.YStart),
                new Point(line1F31.XStart, line1F31.YStart));
            var p2L5F31 = Point.CenterPointInImage(new Point(line3F31.XEnd, line3F31.YEnd),
                new Point(line1F31.XEnd, line1F31.YEnd));
            var l5F31 = new Line(p1L5F31.ImageX, p1L5F31.ImageY, p2L5F31.ImageX, p2L5F31.ImageY);
            
            var line2F31 = findLineManager.GetLine("31.topRight").SortUpDown();
            var line4F31 = findLineManager.GetLine("31.bottomLeft").SortUpDown();
            var p1L6F31 = Point.CenterPointInImage(new Point(line2F31.XStart, line2F31.YStart),
                new Point(line4F31.XStart, line4F31.YStart));
            var p2L6F31 = Point.CenterPointInImage(new Point(line2F31.XEnd, line2F31.YEnd),
                new Point(line4F31.XEnd, line4F31.YEnd));
            var l6F31 = new Line(p1L6F31.ImageX, p1L6F31.ImageY, p2L6F31.ImageX, p2L6F31.ImageY);

            var pMeasureF31 = l5F31.Intersect(l6F31);
            var lineHF31 = coordinateSolver.TranslateLineInWorldUnit(25.362, lineTopBase);
            var lineVF31 = coordinateSolver.TranslateLineInWorldUnit(-9.299, lineLeftBase);
            var pIdealF31 = lineHF31.Intersect(lineVF31);
            var valueF31 = 2 * coordinateSolver.PointPointDistanceInWorld(pIdealF31, pMeasureF31);

            #endregion

            #region 32

            var lineF32Top = lineF21Top;
            var lineF32Bottom = coordinateSolver.TranslateLineInWorldUnit(30.1, lineTopBase, true);

            var lineF32Left = lineF23Left;
            var lineF32Right = lineF23Right;
            var lineF32IdealHorizontal = coordinateSolver.TranslateLineInWorldUnit(25.392, lineTopBase, true);
            var lineF32IdealVertical = coordinateSolver.TranslateLineInWorldUnit(-9.299, lineLeftBase, true);

            var pTopLeftF32 = findLineManager.GetLine("23top").Intersect(lineF32Left);
            var pTopRightF32 = findLineManager.GetLine("23top").Intersect(lineF32Right);
            var pBottomLeftF32 = findLineManager.GetLine("23bottom").Intersect(lineF32Left);
            var pBottomRightF32 = findLineManager.GetLine("23bottom").Intersect(lineF32Right);
            var pLeftTopF32 = findLineManager.GetLine("21left").Intersect(lineF32Top);
            var pLeftBottomF32 = findLineManager.GetLine("21left").Intersect(lineF32Bottom);
            var pRightTopF32 = findLineManager.GetLine("21right").Intersect(lineF32Top);
            var pRightBottomF32 = findLineManager.GetLine("21right").Intersect(lineF32Bottom);
            
            var pF32Top = Point.CenterPointInImage(pLeftTopF32, pRightTopF32);
            var pF32Bottom = Point.CenterPointInImage(pLeftBottomF32, pRightBottomF32);
            var lineF32Vertical = new Line(pF32Top.ImageX, pF32Top.ImageY, pF32Bottom.ImageX, pF32Bottom.ImageY);

            var pF32Left = Point.CenterPointInImage(pTopLeftF32, pBottomLeftF32);
            var pF32Right = Point.CenterPointInImage(pTopRightF32, pBottomRightF32);
            var lineF32Horizontal = new Line(pF32Left.ImageX, pF32Left.ImageY, pF32Right.ImageX, pF32Right.ImageY);

            var pF32Ideal = lineF32IdealHorizontal.Intersect(lineF32IdealVertical);

            var pF32 = lineF32Horizontal.Intersect(lineF32Vertical);
            
            var valueF32 = 2 * coordinateSolver.PointPointDistanceInWorld(pF32Ideal, pF32);

            #endregion

            #region 33

            var lineF33Left = lineF26Left;
            var lineF33Right = lineF26Right;

            var pM1F33 = Point.CenterPointInImage(pLeftTopF26, pLeftBottomF26);
            var pM2F33 = Point.CenterPointInImage(pRightTopF26, pRightBottomF26);
            var l4F33 = new Line(pM1F33.ImageX, pM1F33.ImageY, pM2F33.ImageX, pM2F33.ImageY);

            var lineLeftF33 = lineF26Left.SortUpDown();
            var lineRightF33 = lineF26Right.SortUpDown();
            var l3p1F33 = Point.CenterPointInImage(lineLeftF33.PointStart, lineRightF33.PointStart);
            var l3p2F33 = Point.CenterPointInImage(lineLeftF33.PointEnd, lineRightF33.PointEnd);
            var l3F33 = new Line(l3p1F33, l3p2F33);

            var pMeasureF33 = l4F33.Intersect(l3F33);

            var lineHF33 = coordinateSolver.TranslateLineInWorldUnit(7.886, lineTopBase);
            var lineVF33 = coordinateSolver.TranslateLineInWorldUnit(-9.299, lineLeftBase);
            var pIdealF33 = lineHF33.Intersect(lineVF33);
            
            var valueF33 = 2 * coordinateSolver.PointPointDistanceInWorld(pIdealF33, pMeasureF33);
            

            #endregion

            #region 123

            var p1F123 = findLineManager.GetLine("123").Intersect(lineF123Left);
            var p2F123 = findLineManager.GetLine("123").Intersect(lineF123Center);
            var p3F123 = findLineManager.GetLine("123").Intersect(lineF123Right);
            
            var valueF123_1 = coordinateSolver.PointLineDistanceInWorld(p1F123, lineTopBase);
            var valueF123_2 = coordinateSolver.PointLineDistanceInWorld(p2F123, lineTopBase);
            var valueF123_3 = coordinateSolver.PointLineDistanceInWorld(p3F123, lineTopBase);
            

            #endregion


            #region Outputs

            Dictionary<string, double> outputs = new Dictionary<string, double>();
            outputs["21_1"] = valueF21Top;
            outputs["21_2"] = valueF21Bottom;
            outputs["23_1"] = valueF23Left;
            outputs["23_2"] = valueF23Right;
            outputs["24_1"] = valueF24;
            outputs["25_1"] = valueF25_1;
            outputs["25_2"] = valueF25_2;
            outputs["26_1"] = valueF26_1;
            outputs["26_2"] = valueF26_2;
            outputs["27_1"] = value27_1;
            outputs["27_2"] = value27_2;
            outputs["28_1"] = valueF28_1;
            outputs["28_2"] = valueF28_2;
            outputs["29_1"] = valueF29_1;
            outputs["29_2"] = valueF29_2;
            outputs["31_1"] = valueF31;
            outputs["32_1"] = valueF32;
            outputs["33_1"] = valueF33;
            outputs["123_1"] = valueF123_1;
            outputs["123_2"] = valueF123_2;
            outputs["123_3"] = valueF123_3;
            
            var graphics = new HalconGraphics()
            {
                CrossesIgnored = findLineManager.CrossesIgnored,
                CrossesUsed = findLineManager.CrossesUsed,
                FindLineRects = findLineManager.FindLineRects,
                LineRegions = findLineManager.LineRegions,
                Edges = HalconHelper.ConcatAll(findLineManager.Edges, edgeLeftTop, edgeLeftBottom, edgeRightTop, edgeRightBottom, leftCircleContour, rightCircleContour),
                PointPointGraphics = coordinateSolver.PointPointDistanceGraphics,
                PointLineGraphics = coordinateSolver.PointLineDistanceGraphics,
//                Image = image
            };
            

            #endregion
            
            return new ImageProcessingResult()
            {
                DataRecorder = new DataRecorder(changeOfBaseInv),
                FaiDictionary = outputs,
                HalconGraphics = graphics
            };
        }
    }
}