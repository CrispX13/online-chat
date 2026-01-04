namespace OnlineChatBackend.DTOs
{
    public class DialogPostDTO
    {
        public int UserKey1 { get; set; }
        public int UserKey2 { get; set; }
        public void Normalize()
        {
            if (UserKey1 > UserKey2)
                (UserKey1, UserKey2) = (UserKey2, UserKey1);
        }

        public DialogPostDTO() { }

        public DialogPostDTO(int UserKey1, int UserKey2)
        {
            this.UserKey1 = UserKey1;
            this.UserKey2 = UserKey2;
        }
    }
}
