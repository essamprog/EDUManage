namespace EduManage.Application.Common
{
    /// <summary>
    /// الهيكل القياسي المعتمد لمسارات التخزين في Cloudinary لمشروع EduManage.
    /// كل المسارات تبدأ بـ "edumanage/" وتتفرع بشكل منطقي حسب نوع الملف.
    /// </summary>
    public static class StoragePaths
    {
        private const string Root = "edumanage";

        // ──────────────────────────────────────────────────────────────
        // USERS (Avatars)
        // edumanage/users/{userId}/avatars/
        // ──────────────────────────────────────────────────────────────

        /// <summary>
        /// مسار صورة البروفايل (Avatar) لأي مستخدم (طالب أو مدرس).
        /// </summary>
        /// <param name="userId">الـ ID الخاص بالمستخدم</param>
        public static string UserAvatars(int userId)
            => $"{Root}/users/{userId}/avatars";

        // ──────────────────────────────────────────────────────────────
        // COURSES
        // ──────────────────────────────────────────────────────────────

        /// <summary>
        /// مسار صور غلاف الكورس (Thumbnail).
        /// edumanage/instructors/{instructorId}/courses/{courseId}/images/
        /// </summary>
        public static string CourseImages(int instructorId, int courseId)
            => $"{Root}/instructors/{instructorId}/courses/{courseId}/images";

        /// <summary>
        /// مسار فيديوهات الكورس (الدروس).
        /// edumanage/instructors/{instructorId}/courses/{courseId}/videos/
        /// </summary>
        public static string CourseVideos(int instructorId, int courseId)
            => $"{Root}/instructors/{instructorId}/courses/{courseId}/videos";

        /// <summary>
        /// مسار ملفات الكورس (PDFs, ZIPs, إلخ).
        /// edumanage/instructors/{instructorId}/courses/{courseId}/materials/
        /// </summary>
        public static string CourseMaterials(int instructorId, int courseId)
            => $"{Root}/instructors/{instructorId}/courses/{courseId}/materials";

        // ──────────────────────────────────────────────────────────────
        // SYSTEM ASSETS
        // edumanage/system/assets/
        // ──────────────────────────────────────────────────────────────

        /// <summary>
        /// مسار ملفات النظام الأساسية (اللوجو، الصور الافتراضية، إلخ).
        /// edumanage/system/assets/
        /// </summary>
        public static string SystemAssets
            => $"{Root}/system/assets";
    }
}
