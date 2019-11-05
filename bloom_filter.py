import hashlib


class BloomFilter:
    def __init__(self):
        self.list = [0] * (2 ** 14)
        self.length = 2 ** 19 - 1
        self.hashes = [hashlib.md5(), hashlib.sha256(), hashlib.sha384(), hashlib.blake2s(), hashlib.blake2b()]

    def __contains__(self, item):
        pre_h = str(item).encode('utf-8')
        for h in self.hashes:
            h.update(pre_h)
            hash_code = h.hexdigest()
            index = int(hash_code, 16) & self.length
            i = index >> 5
            rest = index & 31
            if self.list[i] & (1 << rest) == 0:
                return False
        return True

    def add(self, item):
        pre_h = str(item).encode('utf-8')
        for h in self.hashes:
            h.update(pre_h)
            hash_code = h.hexdigest()
            index = int(hash_code, 16) & self.length
            i = index >> 5
            rest = index & 31
            self.list[i] |= (1 << rest)


if __name__ == '__main__':
    b = BloomFilter()
    top = 1 << 20
    s = set()
    for v in range(10000):
        b.add(v)
        s.add(v)

