from docx import Document

def count_gifs(docx_file):
    doc = Document(docx_file)
    count = 0
    for rel in doc.part.rels:
        if "image" in rel.reltype and rel.target_ref.endswith(".gif"):
            count += 1
    return count

# Replace 'your_document.docx' with the path to your Word document
docx_file = 'H:\PortableISO.docx'
gif_count = count_gifs(docx_file)
print("Number of GIFs in the document:", gif_count)
