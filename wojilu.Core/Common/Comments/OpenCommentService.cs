﻿using System;
using System.Collections.Generic;
using System.Text;
using wojilu.Common.Msg.Interface;
using wojilu.Common.Msg.Service;
using wojilu.Members.Users.Domain;
using wojilu.Common.Msg.Enum;

namespace wojilu.Common.Comments {


    public class OpenCommentService {

        public INotificationService nfService { get; set; }

        public OpenCommentService() {
            nfService = new NotificationService();
        }

        public DataPage<OpenComment> GetByUrlDesc( String url ) {

            DataPage<OpenComment> datas = OpenComment.findPage( "TargetUrl='" + strUtil.SqlClean( url, 50 ) + "' and ParentId=0" );

            datas.Results = addSubList( datas.Results, true );

            return datas;
        }

        public DataPage<OpenComment> GetByUrlAsc( String url ) {

            DataPage<OpenComment> datas = OpenComment.findPage( "TargetUrl='" + strUtil.SqlClean( url, 50 ) + "' and ParentId=0 order by Id asc" );

            datas.Results = addSubList( datas.Results, false );

            return datas;
        }

        //----------------------------------------------------------------------------------------------


        private List<OpenComment> addSubList( List<OpenComment> list, Boolean isDesc ) {

            String subIds = "";
            foreach (OpenComment c in list) {
                if (isDesc) {
                    subIds = strUtil.Join( subIds, c.LastReplyIds, "," );
                }
                else {
                    subIds = strUtil.Join( subIds, c.FirstReplyIds, "," );
                }
            }

            subIds = subIds.Trim().TrimStart( ',' ).TrimEnd( ',' );
            if (strUtil.IsNullOrEmpty( subIds )) return list;

            List<OpenComment> totalSubList = OpenComment.find( "Id in (" + subIds + ")" ).list();
            foreach (OpenComment c in list) {
                c.SetReplyList( getSubListFromTotal( c, totalSubList ) );
            }

            return list;
        }

        private List<OpenComment> getSubListFromTotal( OpenComment parent, List<OpenComment> totalSubList ) {

            List<OpenComment> results = new List<OpenComment>();
            int iCount = 0;
            foreach (OpenComment c in totalSubList) {

                if (iCount >= OpenComment.subCacheSize) break;

                if (c.ParentId == parent.Id) {
                    results.Add( c );
                    iCount = iCount + 1;
                }
            }

            return results;
        }

        //----------------------------------------------------------------------------------------------

        public Result Create( OpenComment c ) {

            Result result = c.insert();
            if (result.IsValid) {
                updateParentReplies( c );
                updateRootTargetReplies( c );
                sendNotifications( c );
                return result;
            }
            else {
                return result;
            }

        }

        private void sendNotifications( OpenComment c ) {

            int parentReceiverId = 0;
            if (c.ParentId > 0) {
                OpenComment p = OpenComment.findById( c.ParentId );
                if (p != null && p.Member != null) {
                    parentReceiverId = sendNotificationsTo( p, c );
                }
            }

            int atUserId = 0;
            if (c.AtId > 0) {
                OpenComment at = OpenComment.findById( c.AtId );
                if (at != null && at.Member != null) {
                    atUserId = sendNotificationsTo( at, c );
                }
            }

            if (c.TargetUserId > 0) {
                sendNotificationToRoot( c, parentReceiverId, atUserId );
            }
        }

        private void sendNotificationToRoot( OpenComment c, int parentReceiverId, int atUserId ) {

            if (c.Member != null && c.Member.Id == c.TargetUserId) return; // 不用给自己发通知
            if (c.TargetUserId == parentReceiverId || c.TargetUserId == atUserId) return; // 已经发过，不用重发

            String msg = c.Author + " 回复了你的 <a href=\"" + c.TargetUrl + "\">" + c.TargetTitle + "</a> ";
            nfService.send( c.TargetUserId, typeof( User ).FullName, msg, NotificationType.Comment );
        }

        private int sendNotificationsTo( OpenComment comment, OpenComment c ) {

            int receiverId = comment.Member.Id;

            if (c.Member != null && c.Member.Id == receiverId) return 0; // 不用给自己发通知

            String msg = c.Author + " 回复了你在 <a href=\"" + comment.TargetUrl + "\">" + comment.TargetTitle + "</a> 的评论";
            nfService.send( receiverId, typeof( User ).FullName, msg, NotificationType.Comment );
            return receiverId;
        }

        private static void updateParentReplies( OpenComment c ) {

            if (c.ParentId == 0) return;

            OpenComment p = OpenComment.findById( c.ParentId );
            if (p == null) {
                c.ParentId = 0;
                c.update();
                return;
            }

            //------------------------------------------------
            p.Replies = OpenComment.count( "ParentId=" + p.Id );

            //-------------------------------------------------
            List<OpenComment> subFirst = OpenComment.find( "ParentId=" + p.Id + " order by Id asc" ).list( OpenComment.subCacheSize );
            List<OpenComment> subLast = OpenComment.find( "ParentId=" + p.Id + " order by Id desc" ).list( OpenComment.subCacheSize );

            p.FirstReplyIds = strUtil.GetIds( subFirst );
            p.LastReplyIds = strUtil.GetIds( subLast );

            p.update();

        }


        public List<OpenComment> GetMore( int parentId, int startId, int replyPageSize, string sort ) {

            String condition = "";

            if (sort == "asc") {
                condition = "ParentId=" + parentId + " and Id>" + startId + " order by Id asc";
            }
            else {
                condition = "ParentId=" + parentId + " and Id<" + startId + " order by Id desc";
            }

            return OpenComment.find( condition ).list( replyPageSize );
        }

        //------------------------------------------------------------------------------------------------------------


        public int GetReplies( String url ) {
            OpenCommentCount objCount = OpenCommentCount.find( "TargetUrl=:url" )
                .set( "url", url )
                .first();
            return objCount == null ? 0 : objCount.Replies;
        }


        private void updateRootTargetReplies( OpenComment c ) {
            int count = OpenComment.find( "TargetUrl=:url" )
                .set( "url", c.TargetUrl )
                .count();

            OpenCommentCount objCount = OpenCommentCount.find( "TargetUrl=:url" )
                .set( "url", c.TargetUrl )
                .first();

            if (objCount == null) {
                insertCommentCount( c, count );
            }
            else {
                updateCommentCount( objCount, count );
            }
        }

        private static void updateCommentCount( OpenCommentCount objCount, int count ) {
            objCount.Replies = count;
            objCount.update();
        }

        private static void insertCommentCount( OpenComment c, int count ) {
            OpenCommentCount objCount = new OpenCommentCount();
            objCount.TargetUrl = c.TargetUrl;
            objCount.DataType = c.TargetDataType;
            objCount.DataId = c.TargetDataId;
            objCount.Replies = count;

            objCount.insert();
        }



    }
}
