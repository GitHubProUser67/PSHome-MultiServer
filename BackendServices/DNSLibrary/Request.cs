using System.Security.Cryptography;
using DNSLibrary.ResourceRecords;
using DNSLibrary.Utils;

namespace DNSLibrary
{
    public class Request : IRequest
    {
        private readonly IList<Question> questions;
        private Header header;
        private readonly IList<IResourceRecord> additional;

        public static Request FromArray(byte[] message)
        {
            var header = Header.FromArray(message);
            var offset = Header.Size;

            return
                header.Response
                || header.QuestionCount == 0
                || header.AnswerRecordCount + header.AuthorityRecordCount > 0
                || header.ResponseCode != ResponseCode.NoError
                ? throw new ArgumentException("Invalid request message")
                : new Request(
                    header,
                    Question.GetAllFromArray(message, offset, header.QuestionCount, out offset),
                    ResourceRecordFactory.GetAllFromArray(
                        message,
                        offset,
                        header.AdditionalRecordCount,
                        out _
                    )
                );
        }

        public Request(Header header, IList<Question> questions, IList<IResourceRecord> additional)
        {
            this.header = header;
            this.questions = questions;
            this.additional = additional;
        }

        public Request()
        {
            questions = [];
            header = new Header();
            additional = [];

            header.OperationCode = OperationCode.Query;
            header.Response = false;
            header.Id = NextRandomId();
        }

        public Request(IRequest request)
        {
            header = new Header();
            questions = [.. request.Questions];
            additional = [.. request.AdditionalRecords];

            header.Response = false;

            Id = request.Id;
            OperationCode = request.OperationCode;
            RecursionDesired = request.RecursionDesired;
        }

        public IList<Question> Questions
        {
            get { return questions; }
        }

        public IList<IResourceRecord> AdditionalRecords
        {
            get { return additional; }
        }

        public int Size
        {
            get { return Header.Size + questions.Sum(q => q.Size) + additional.Sum(a => a.Size); }
        }

        public int Id
        {
            get { return header.Id; }
            set { header.Id = value; }
        }

        public OperationCode OperationCode
        {
            get { return header.OperationCode; }
            set { header.OperationCode = value; }
        }

        public bool RecursionDesired
        {
            get { return header.RecursionDesired; }
            set { header.RecursionDesired = value; }
        }

        public byte[] ToArray()
        {
            UpdateHeader();
            var result = new ByteStream(Size);

            result
                .Append(header.ToArray())
                .Append(questions.Select(q => q.ToArray()))
                .Append(additional.Select(a => a.ToArray()));

            return result.ToArray();
        }

        public override string ToString()
        {
            UpdateHeader();

            return ObjectStringifier
                .New(this)
                .Add(nameof(Header), header)
                .Add(nameof(Questions), nameof(AdditionalRecords))
                .ToString();
        }

        private void UpdateHeader()
        {
            header.QuestionCount = questions.Count;
            header.AdditionalRecordCount = additional.Count;
        }

        private static ushort NextRandomId() =>
            RandomNumberGenerator.GetBytes(sizeof(ushort)) switch
            {
                var bytes => BitConverter.ToUInt16(bytes),
            };
    }
}
